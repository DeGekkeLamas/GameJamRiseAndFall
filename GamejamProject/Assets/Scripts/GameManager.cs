using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public RectTransform fadeEff;
    public float fadeSpeed = 20;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) StartCoroutine(BackwardTimeAnimation());
        if (Input.GetKeyDown(KeyCode.E)) StartCoroutine(ForwardTimeAnimation());
    }

    IEnumerator ForwardTimeAnimation()
    {
        for (int i = 0; i < 1600/fadeSpeed; i++)
        {
            fadeEff.anchoredPosition = new Vector2(0f, (500 - -100 - i * fadeSpeed) / 2) * fadeEff.parent.localScale.x;
            fadeEff.sizeDelta = new Vector2(0, -(500 + -100 - i * fadeSpeed)) * fadeEff.parent.localScale.x;
            //fadeEff.sizeDelta -= new Vector2(0, fadeSpeed);
            yield return new WaitForFixedUpdate();
        }

        ForwardTime();
        
        for (int i = 0; i < 1600/fadeSpeed; i++)
        {
            fadeEff.anchoredPosition = new Vector2(0f, (-600 - -100 - i * -fadeSpeed) / 2) * fadeEff.parent.localScale.x;
            fadeEff.sizeDelta = new Vector2(0, -(-600 + -100 - i * -fadeSpeed)) * fadeEff.parent.localScale.x;
            //fadeEff.sizeDelta -= new Vector2(0, fadeSpeed);
            yield return new WaitForFixedUpdate();
        }

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
        for (int i = 0; i < 1600 / fadeSpeed; i++)
        {
            fadeEff.anchoredPosition = new Vector2(0f, (500 - -100 - i * fadeSpeed) / 2) * fadeEff.parent.localScale.x;
            fadeEff.sizeDelta = new Vector2(0, -(500 + -100 - i * fadeSpeed)) * fadeEff.parent.localScale.x;
            //fadeEff.sizeDelta -= new Vector2(0, fadeSpeed);
            yield return new WaitForFixedUpdate();
        }

        BackwardTime();

        for (int i = 0; i < 1600 / fadeSpeed; i++)
        {
            fadeEff.anchoredPosition = new Vector2(0f, (-600 - -100 - i * -fadeSpeed) / 2) * fadeEff.parent.localScale.x;
            fadeEff.sizeDelta = new Vector2(0, -(-600 + -100 - i * -fadeSpeed)) * fadeEff.parent.localScale.x;
            //fadeEff.sizeDelta -= new Vector2(0, fadeSpeed);
            yield return new WaitForFixedUpdate();
        }

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
