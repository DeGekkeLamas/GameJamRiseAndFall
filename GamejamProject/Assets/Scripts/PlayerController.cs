using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    private new Rigidbody rigidbody;
    public float walkSpeed = 1;
    public float jumpForce = 1;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Walking
        transform.position += walkSpeed * Time.deltaTime * new Vector3(Input.GetAxis("Horizontal"), 0, 0);

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            /// Collision testing
            bool collisionLeft = Physics.Raycast(
                this.transform.position - .49f * new Vector3(-this.transform.localScale.x, this.transform.localScale.y, 0),
                Vector3.down, .1f);
            bool collisionRight = Physics.Raycast(
                this.transform.position - .49f * new Vector3(this.transform.localScale.x, this.transform.localScale.y, 0),
                Vector3.down, .1f);
            /// Add velocity
            if (collisionLeft || collisionRight)
            rigidbody.AddForce(Vector3.up * jumpForce);
        }
    }
}
