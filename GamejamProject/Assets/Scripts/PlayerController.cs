using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    [HideInInspector] public TreeStages onTree;

    private new Rigidbody rigidbody;
    public float walkSpeed = 1;
    public float jumpForce = 1;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        Instance = this;
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

    /// <summary>
    /// Set tree value after landing on one
    /// </summary>
    private void OnCollisionStay(Collision collision)
    {
        GameObject obj = collision.gameObject;
        while(obj.transform.parent != null)
        {
            if (obj.TryGetComponent(out TreeStages tree)) 
            {
                onTree = tree;
                break;
            }
            obj = obj.transform.parent.gameObject;
        }
        //Debug.Log($"new collision tree = {onTree.gameObject}");
    }

    /// <summary>
    /// Remove tree from value after leaving tree
    /// </summary>
    private void OnCollisionExit(Collision collision)
    {
        onTree = null;
        print("bye bye platform");
    }
}
