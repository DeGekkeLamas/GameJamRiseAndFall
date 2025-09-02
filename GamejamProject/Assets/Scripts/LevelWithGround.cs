using UnityEngine;

public class LevelWithGround : MonoBehaviour
{
    public float groundHeight = 0;
    private void Start()
    {
        LevelObject();
    }
    [NaughtyAttributes.Button]
    void LevelObject()
    {
        this.transform.position = new(
            this.transform.position.x, 
            groundHeight + .5f * this.transform.lossyScale.y, 
            this.transform.position.z);
    }
}
