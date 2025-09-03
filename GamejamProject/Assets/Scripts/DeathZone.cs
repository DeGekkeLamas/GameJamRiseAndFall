using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.TryGetComponent<PlayerController>(out _))
        {
            GameManager.instance.GameOver();
        }
    }
}
