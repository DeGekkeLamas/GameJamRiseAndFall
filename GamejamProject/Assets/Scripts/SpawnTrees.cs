using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SpawnTrees : MonoBehaviour
{
    public List<GameObject> prefabsOfTrees;
    public List<GameObject> spawnedTrees = new();

    public int treeCount;
    public int minDistance;
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
        for (int i = 0; i < treeCount; i++)
        {
            Vector2 pos = PickRandomPosInArea(spawnableAreaMin, spawnableAreaMax, chosenPositions.ToArray());
            chosenPositions.Add(pos);
            int randomTreeType = Random.Range(0, prefabsOfTrees.Count);
            Vector3 position = new Vector3(pos.x, 0, pos.y);
            spawnedTrees.Add(Instantiate(prefabsOfTrees[randomTreeType], position, Quaternion.identity, treeContainer));
        }
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

    private void ClearTrees()
    {
        foreach (var tree in spawnedTrees)
        {
            DestroyImmediate(tree);
        }
        spawnedTrees.Clear();
    }
}
