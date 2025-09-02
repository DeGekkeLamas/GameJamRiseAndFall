
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnTrees : MonoBehaviour
{
    public List<GameObject> prefabsOfTrees;
    public List<GameObject> spawnedTrees = new();
    public List<Vector2> aditionalPositionsToCheckConnection;
    public int treeCount;
    public int minDistance;
    public int maxDistance;
    public Transform treeContainer;
    public Vector2 spawnableAreaMin;
    public Vector2 spawnableAreaMax;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateTrees();
    }

    [ContextMenu("GenerateTrees")]
    public void GenerateTrees()
    {
        ClearTrees();
        List<Vector2> chosenPositions = new();
        do {
            Vector2 pos = PickRandomPosInArea(spawnableAreaMin, spawnableAreaMax, chosenPositions.ToArray());
            chosenPositions.Add(pos);
            int randomTreeType = Random.Range(0, prefabsOfTrees.Count);
            Vector3 position = new Vector3(pos.x, 0, pos.y);
            spawnedTrees.Add(Instantiate(prefabsOfTrees[randomTreeType], position, Quaternion.identity, treeContainer));
        } while (TestSmallestDistance(chosenPositions.ToArray()));
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
                if (distance < minDistance)
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
            if (closest > maxDistance)
            {
                return true;
            }
        }

        return false;
    }

    

}
