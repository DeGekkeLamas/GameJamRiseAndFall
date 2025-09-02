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

        if (PlayerController.Instance.onTree == this) StartCoroutine(RaisePlayerWithTree());
    }

    public void BackwardTime()
    {
        currentState--;

        DestroyCurrentTree();
        currentTree = Instantiate(treeStages[ClampCurrentState()],
            this.transform.position, Quaternion.identity, this.transform);

        if (PlayerController.Instance.onTree == this) StartCoroutine(RaisePlayerWithTree());
    }

    int ClampCurrentState()
    {
        return Mathf.Clamp(currentState, 0, treeStages.Length-1);
    }
    IEnumerator RaisePlayerWithTree()
    {
        yield return null;
        if (!transform.GetChild(0).TryGetComponent<Collider>(out _)) yield break;

        print("raised player");
        // Get tallest point of tree
        float highest = float.MinValue;
        for(int i = 0; i < this.transform.GetChild(0).childCount; i++)
        {
            float point = this.transform.GetChild(0).GetChild(i).GetComponent<Collider>().ClosestPoint(new(0, 1000, 0)).y;
            if (point > highest) highest = point;
        }
        // Set player pos
        Vector3 playerPos = PlayerController.Instance.transform.position;
        PlayerController.Instance.transform.position = new(playerPos.x, 
            highest + PlayerController.Instance.transform.lossyScale.y * .5f, playerPos.z);
        PlayerController.Instance.onTree = this;
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
