using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) BackwardTime();
        if (Input.GetKeyDown(KeyCode.E)) ForwardTime();
    }

    IEnumerator ForwardTimeAnimation()
    {


        ForwardTime();
        yield return new();
    }

    [Button("Forward time", EButtonEnableMode.Playmode)]
    void ForwardTime()
    {
        foreach(TreeStages tree in TreeStages.allTrees)
        {
            tree.ForwardTime();
        }
    }
    IEnumerator BackwardTimeAnimation()
    {


        BackwardTime();
        yield return new();
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
