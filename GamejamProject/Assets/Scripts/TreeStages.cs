using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeStages : MonoBehaviour
{
    public GameObject[] treeStages;
    public int currentState;
    GameObject currentTree;

    public static List<TreeStages> allTrees = new();
    void Awake()
    {
        allTrees.Add(this);
        currentTree = this.transform.GetChild(0).gameObject;
    }

    private void OnValidate()
    {
        if (treeStages.Length < 1) return;

        currentState = ClampCurrentState();

        DestroyCurrentTree();
        currentTree = Instantiate(treeStages[currentState],
            this.transform.position, Quaternion.identity, this.transform);
    }

    public void ForwardTime()
    {
        currentState++;

        DestroyCurrentTree();
        currentTree = Instantiate(treeStages[ClampCurrentState()],
            this.transform.position, Quaternion.identity, this.transform);
    }

    public void BackwardTime()
    {
        currentState--;

        DestroyCurrentTree();
        currentTree = Instantiate(treeStages[ClampCurrentState()], 
            this.transform.position, Quaternion.identity, this.transform);
    }

    int ClampCurrentState()
    {
        return Mathf.Clamp(currentState, 0, treeStages.Length-1);
    }

    void DestroyCurrentTree()
    {
        #if UNITY_EDITOR
        if(!UnityEditor.EditorApplication.isPlaying)
        {
            UnityEditor.EditorApplication.delayCall += () => DestroyImmediate(currentTree);
        }
        else
        {
            #endif
            Destroy(currentTree);
        #if UNITY_EDITOR
        }
        #endif
    }
}
