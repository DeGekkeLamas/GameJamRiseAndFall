using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SpawnTrees : MonoBehaviour
{
    [Header("Tree Prefabs & Settings")]
    public List<GameObject> prefabsOfTrees;
    public int minDistanceBetweenTrees;
    public int maxDistanceBetweenTrees;
    public Transform treeContainer;

    [Header("Spawn Area")]
    public Vector2 spawnableAreaMin;
    public Vector2 spawnableAreaMax;

    [Header("Connection Points")]
    [Tooltip("Exactly two positions: start and end")]
    public List<Vector2> aditionalPositionsToCheckConnection;

    [Header("Graph & Generation Data")]
    [SerializeField] private List<GraphNode> graph = new List<GraphNode>();
    private List<Vector2> generatedPositions = new List<Vector2>();
    private List<int> generatedTreeTypes = new List<int>();
    private List<GameObject> spawnedTrees = new List<GameObject>();

    [Header("Jump Distance Settings")]
    public List<FloatListWrapper> jumpDistanceSettings = new List<FloatListWrapper>();

    GraphNode startNode;
    GraphNode endNode;

    [ContextMenu("GenerateTrees")]
    public void GenerateTrees()
    {
        // 1) Clear any existing trees & data
        ClearTrees();
        generatedPositions.Clear();
        generatedTreeTypes.Clear();
        graph.Clear();

        // 2) Seed graph with the two fixed connection nodes
        startNode = new GraphNode(aditionalPositionsToCheckConnection[0])
        {
            startingGrowthStage = 1 // or pick a stage if you like
        };
        endNode = new GraphNode(aditionalPositionsToCheckConnection[1])
        {
            startingGrowthStage = 1
        };
        graph.Add(startNode);
        graph.Add(endNode);

        // 3) Incrementally add random trees until constraints pass
        bool needsMore;
        do
        {
            // a) Pick a valid new position
            Vector2 pos = PickRandomPosInArea(
                spawnableAreaMin,
                spawnableAreaMax,
                generatedPositions.ToArray()
            );

            // b) Record position & tree type for later instantiation
            generatedPositions.Add(pos);
            int randomTreeType = Random.Range(0, prefabsOfTrees.Count);
            generatedTreeTypes.Add(randomTreeType);

            // c) Create a new graph node, assign growth stage
            var newNode = new GraphNode(pos)
            {
                startingGrowthStage = Random.Range(0, 4)
            };

            // d) Link newNode to all existing graph nodes if TestValidPath passes
            foreach (var existing in graph)
            {
                if (TestValidPath(existing, newNode)) { existing.Neighbors.Add(newNode); newNode.Neighbors.Add(existing); }
                if (TestValidPath(newNode, existing)) { newNode.Neighbors.Add(existing); existing.Neighbors.Add(newNode);}

            }

            // e) Add newNode to the graph
            graph.Add(newNode);

            // f) Check your two stop conditions:
            //    1) No pair exceeding maxDistanceBetweenTrees
            //    2) A valid BFS path from startNode to endNode
            bool tooSparse = TestSmallestDistance(generatedPositions.ToArray());
            bool noPathYet = !PathExists(startNode, endNode);
            needsMore = tooSparse || noPathYet;

        } while (needsMore);

        // 4) Finally instantiate all trees at once
        for (int i = 0; i < generatedPositions.Count; i++)
        {
            Vector2 p = generatedPositions[i];
            int prefabIdx = generatedTreeTypes[i];
            Vector3 worldP = new Vector3(p.x, 0, p.y);

            var tree = Instantiate(
                prefabsOfTrees[prefabIdx],
                worldP,
                Quaternion.identity,
                treeContainer
            );
            spawnedTrees.Add(tree);
        }
    }


    private Vector2 PickRandomPosInArea(
        Vector2 min,
        Vector2 max,
        Vector2[] chosenPositions
    )
    {
        while (true)
        {
            Vector2 pos = new Vector2(
                Random.Range(min.x, max.x),
                Random.Range(min.y, max.y)
            );

            // enforce minDistanceBetweenTrees
            bool ok = true;
            foreach (var other in chosenPositions)
            {
                if (Vector2.Distance(other, pos) < minDistanceBetweenTrees)
                {
                    ok = false;
                    break;
                }
            }

            if (ok)
                return pos;
        }
    }

    [ContextMenu("TestPath")]
    public void TestPath()
    {
        print(PathExists(startNode, endNode));

    }


    [ContextMenu("ClearTrees")]
    public void ClearTrees()
    {
        foreach (var t in spawnedTrees)
            DestroyImmediate(t);
        spawnedTrees.Clear();
    }


    private bool TestSmallestDistance(Vector2[] chosenPositions)
    {
        if (chosenPositions.Length <= 1)
            return true;

        var all = chosenPositions.Concat(aditionalPositionsToCheckConnection).ToArray();
        foreach (var a in all)
        {
            float closest = float.MaxValue;
            foreach (var b in chosenPositions)
            {
                if (a == b) continue;
                closest = Mathf.Min(closest, Vector2.Distance(a, b));
            }

            if (closest > maxDistanceBetweenTrees)
                return true;
        }
        return false;
    }


    private bool TestValidPath(GraphNode from, GraphNode to)
    {
        float dist = Vector2.Distance(from.Position, to.Position);
        // example: you allow up to N jumps depending on growth stage
        for (int i = 0; i < 5 - from.startingGrowthStage; i++)
        {
            float jumpLimit = jumpDistanceSettings[from.startingGrowthStage]
                                      .values[to.startingGrowthStage];
            if (dist < jumpLimit)
                return true;
        }
        return false;
    }


    private bool PathExists(GraphNode start, GraphNode goal)
    {
        var visited = new HashSet<GraphNode> { start };
        var queue = new Queue<GraphNode>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            if (node == goal)
                return true;

            foreach (var nei in node.Neighbors)
            {
                if (visited.Add(nei))
                    queue.Enqueue(nei);
            }
        }
        return false;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (var node in graph)
        {
            Vector3 a = new Vector3(node.Position.x, 0, node.Position.y);
            foreach (var nei in node.Neighbors)
            {
                Vector3 b = new Vector3(nei.Position.x, 0, nei.Position.y);
                Gizmos.DrawLine(a, b);
            }
        }
    }
}


// Graph node definition remains the same

public class GraphNode
{
    public Vector2 Position;
    public int startingGrowthStage;

    
    public List<GraphNode> Neighbors = new List<GraphNode>();

    public GraphNode(Vector2 pos)
    {
        Position = pos;
    }
}


// Wrapper for your jump distance matrix
[System.Serializable]
public class FloatListWrapper
{
    public string growthStageName;
    public List<float> values = new List<float>();
}
