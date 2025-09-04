using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SpawnTrees : MonoBehaviour
{
    [Header("Tree Prefabs & Settings")]
    public GameObject scriptedTreeprefab;
    public int minDistanceBetweenTrees;
    public Transform treeContainer;

    [Header("Spawn Area")]
    public Vector2 spawnableAreaMin;
    public Vector2 spawnableAreaMax;
    public bool displayArea;

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

    private void Start()
    {
        GenerateTrees();
    }


    [ContextMenu("GenerateTrees")]
    public void GenerateTrees()
    {
        float startTime = Time.realtimeSinceStartup;
        // 1) Clear any existing trees & data
        ClearTrees();
        generatedPositions.Clear();
        generatedTreeTypes.Clear();
        graph.Clear();

        // 2) Seed graph with the two fixed connection nodes
        startNode = new GraphNode(aditionalPositionsToCheckConnection[0], 0);
        endNode = new GraphNode(aditionalPositionsToCheckConnection[1], 0);
        
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
            int randomTreeType = Random.Range(0, scriptedTreeprefab.GetComponent<TreeStages>().treeStages.Length);
            

            // c) Create a new graph node, assign growth stage
            var newNode = new GraphNode(pos, randomTreeType);
            graph.Add(newNode);

            // d) Link newNode to all existing graph nodes if TestValidPath passes
            foreach (var existing in graph)
            {
                if (TestValidPath(existing, newNode))  existing.Neighbors.Add(newNode); 
                if (TestValidPath(newNode, existing))  newNode.Neighbors.Add(existing); 
            }

            // e) Add newNode to the graph
            

            // f) Check your two stop conditions:
            //    1) No pair exceeding maxDistanceBetweenTrees
            //    2) A valid BFS path from startNode to endNode
            //bool tooSparse = TestSmallestDistance(generatedPositions.ToArray());
            bool noPathYet = !PathExists(startNode, endNode);
            needsMore = noPathYet;
            if ((int)(startTime - Time.realtimeSinceStartup) < -10)
            {
                print(startTime);
                print(Time.realtimeSinceStartup);
                print((int)(startTime - Time.realtimeSinceStartup));
                Debug.LogWarning("too much time has passed. abborting algorithm");
                break;
            }

        } while (needsMore);

        // 4) Finally instantiate all trees at once
        for (int i = 0; i < graph.Count; i++)
        {
            Vector2 p = graph[i].Position;
            int prefabIdx = graph[i].startingGrowthStage;
            Vector3 worldP = new Vector3(p.x, 0, p.y);

            var tree = Instantiate(
                scriptedTreeprefab,
                worldP,
                Quaternion.identity,
                treeContainer
            );
            spawnedTrees.Add(tree);
            TreeStages treeScript = tree.GetComponent<TreeStages>();
            treeScript.currentState = prefabIdx;
            treeScript.UpdateTreeModel();

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
        graph.Clear();
        foreach (var t in spawnedTrees)
            DestroyImmediate(t);
        spawnedTrees.Clear();
    }

    private bool TestValidPath(GraphNode from, GraphNode to)//change to take plant growth in mind
    {
        float dist = Vector2.Distance(from.Position, to.Position);
        int max = Mathf.Max(from.startingGrowthStage, to.startingGrowthStage);
        int min = Mathf.Min(from.startingGrowthStage, to.startingGrowthStage);
        int goingBack = 0 - min;
        int goingForward = 6-max;

        for (int i = goingBack; i < goingForward; i++)
        {
            float jumpLimit = jumpDistanceSettings[from.startingGrowthStage + i].values[to.startingGrowthStage + i];

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
    private void OnValidate()
    {
        StopAllCoroutines();
        StartCoroutine(ShowFillArea());
        // Set pivot to tell which object is which area
        this.transform.position = new RectInt(Vector2Int.RoundToInt(spawnableAreaMin), 
            Vector2Int.RoundToInt(spawnableAreaMax - spawnableAreaMin)).center;
    }
    IEnumerator ShowFillArea()
    {
        while (displayArea)
        {
            AlgorithmsUtils.DebugRectInt(
                new(Vector2Int.RoundToInt(spawnableAreaMin), Vector2Int.RoundToInt(spawnableAreaMax - spawnableAreaMin)), Color.red);

            foreach(Vector2 point in aditionalPositionsToCheckConnection)
            {
                DebugExtension.DebugWireSphere(new(point.x, 0, point.y), Color.blue, 1);
            }

            yield return null;
        }
    }
}


// Graph node definition remains the same

public class GraphNode
{
    public Vector2 Position;
    public int startingGrowthStage;

    
    public List<GraphNode> Neighbors = new List<GraphNode>();

    public GraphNode(Vector2 pos, int startGrowthStage)
    {
        startingGrowthStage = startGrowthStage;
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
