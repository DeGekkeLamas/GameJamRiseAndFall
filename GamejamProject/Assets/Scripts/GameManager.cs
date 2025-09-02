using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) BackwardTime();
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) ForwardTime();
    }

    [Button("Forward time", EButtonEnableMode.Playmode)]
    void ForwardTime()
    {
        foreach(TreeStages tree in TreeStages.allTrees)
        {
            tree.ForwardTime();
        }
    }

    [Button("Backward time", EButtonEnableMode.Playmode)]
    void BackwardTime()
    {
        foreach(TreeStages tree in TreeStages.allTrees)
        {
            tree.BackwardTime();
        }
    }
}
