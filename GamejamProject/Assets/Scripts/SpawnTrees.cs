
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SpawnTrees : MonoBehaviour
{

    public List<GameObject> prefabsOfTrees;
    public List<GameObject> spawnedTrees = new();
    public List<Vector2> aditionalPositionsToCheckConnection;
    public List<Vector2> generatedPositions = new();
    [SerializeField] public List<GraphNode> graph = new List<GraphNode>();
    public int treeCount;
    public int minDistanceBetweenTrees;
    public int maxDistanceBetweenTrees;
    public Transform treeContainer;
    public Vector2 spawnableAreaMin;
    public Vector2 spawnableAreaMax;
    [SerializeField] public List<FloatListWrapper> jumpDistanceSettings = new List<FloatListWrapper>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateTrees();
    }

    [ContextMenu("GenerateTrees")]
    public void GenerateTrees()
    {
        //bool generationDone = false;
        ClearTrees();
        generatedPositions.Clear();
        do {
            Vector2 pos = PickRandomPosInArea(spawnableAreaMin, spawnableAreaMax, generatedPositions.ToArray());
            generatedPositions.Add(pos);
            int randomTreeType = Random.Range(0, prefabsOfTrees.Count);
            Vector3 position = new Vector3(pos.x, 0, pos.y);
            spawnedTrees.Add(Instantiate(prefabsOfTrees[randomTreeType], position, Quaternion.identity, treeContainer));
            

        } while (TestSmallestDistance(generatedPositions.ToArray()) || !TestPathThroughTrees(generatedPositions.ToList()));
    }

    private Vector2 PickRandomPosInArea(Vector2 min, Vector2 max, Vector2[] chosenPositions)
    {
        bool goodPos = false;
        Vector2 pos = Vector2.zero;
        do
        {
            goodPos = true;
            pos = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
            foreach (var position in chosenPositions)
            {
                float distance = Vector2.Distance(position, pos);
                if (distance < minDistanceBetweenTrees)
                {
                    print("badPos");
                    goodPos = false;
                    break;
                }

            }
        } while (!goodPos);
        return new Vector2(pos.x, pos.y);
    }

    [ContextMenu("ClearTrees")]
    public void ClearTrees()
    {
        foreach (var tree in spawnedTrees)
        {
            DestroyImmediate(tree);
        }
        spawnedTrees.Clear();
    }

    [ContextMenu("CheckGraph")]
    public void CheckGraph()
    {
        bool found = TestPathThroughTrees(generatedPositions.ToList());
        print(found);
    }


    private bool TestSmallestDistance(Vector2[] chosenPositions)
    {
        if (chosenPositions.Length == 1) return true;

        Vector2[] allPositionsToTest = chosenPositions.Concat(aditionalPositionsToCheckConnection).ToArray();
        foreach (var pos1 in allPositionsToTest)
        {
            float closest = float.MaxValue;
            foreach (var pos2 in chosenPositions)
            {
                if (pos1 == pos2)
                {
                    continue;
                }
                float dist = Vector2.Distance(pos1, pos2);
                if (dist < closest) closest = dist;
            }
            if (closest > maxDistanceBetweenTrees)
            {
                return true;
            }
        }

        return false;
    }

    

    List<GraphNode> BuildGraph(List<Vector2> allPositions)
    {
        var nodes = allPositions.Select(p => new GraphNode(p)).ToList();

        foreach (var item in nodes)
        {
            item.startingGrowthStage = Random.Range(0, 4);
        }

        int n = nodes.Count;
        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                if (TestValidPath(nodes[i], nodes[j]))
                {
                    nodes[i].Neighbors.Add(nodes[j]);
                }
                if (TestValidPath(nodes[j], nodes[i]))
                {
                    nodes[j].Neighbors.Add(nodes[i]);
                }
            }
        }

        return nodes;
    }

    bool TestValidPath(GraphNode from, GraphNode to)
    {
        float dist = Vector2.Distance(from.Position, to.Position);
        for (int i = 0; i < 5 - from.startingGrowthStage; i++)
        {
            float distanceSettings = jumpDistanceSettings[from.startingGrowthStage].values[to.startingGrowthStage];
            if (dist < distanceSettings) return true;
        }
        return false;
    }

    bool PathExists(GraphNode start, GraphNode goal)
    {
        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();

        visited.Add(start);
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            if (node == goal)
                return true;

            foreach (var neighbor in node.Neighbors)
            {
                if (visited.Add(neighbor))
                    queue.Enqueue(neighbor);
            }
        }

        return false;
    }

    private bool TestPathThroughTrees(List<Vector2> treePositions)
    {
        // assume aditionalPositionsToCheckConnection has exactly two entries: start & end
        var allPositions = new List<Vector2>(treePositions);
        allPositions.AddRange(aditionalPositionsToCheckConnection);

        // 2. Build graph
        float threshold = maxDistanceBetweenTrees;
        graph = BuildGraph(allPositions);

        // 3. Locate start/end nodes
        GraphNode startNode = graph.Single(n => n.Position == aditionalPositionsToCheckConnection[0]);
        GraphNode endNode = graph.Single(n => n.Position == aditionalPositionsToCheckConnection[1]);

        // 4. Run BFS
        bool connected = PathExists(startNode, endNode);
        Debug.Log($"Path from {startNode.Position} to {endNode.Position}? {connected}");
        return connected;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (var item in graph)
        {
            foreach (var item2 in item.Neighbors)
            {
                Vector3 pos1 = new Vector3(item.Position.x, 0, item.Position.y);
                Vector3 pos2 = new Vector3(item2.Position.x, 0, item2.Position.y);
                Gizmos.DrawLine(pos1, pos2);
            }
                
        }
    }
}

//graph stuff
[SerializeField]
public class GraphNode
{
    public Vector2 Position;
    public int startingGrowthStage;
    [SerializeField] public List<GraphNode> Neighbors = new List<GraphNode>();

    public GraphNode(Vector2 pos)
    {
        
        Position = pos;
    }
}

//listWrapper
[System.Serializable] public class FloatListWrapper
{
    public string growthStageName;
    public List<float> values = new List<float>();
}