using System.Security.Cryptography.X509Certificates;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    [HideInInspector] public TreeStages onTree;

    new Rigidbody rigidbody;
    new CameraController camera;
    public Transform model;
    public float walkSpeed = 1;
    public float jumpForce = 1;

    public Animator animator;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        Instance = this;
        camera = Camera.main.transform.parent.GetComponent<CameraController>();
    }

    void Update()
    {
        // Walking
        Vector3 movementDir = new(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical"));
        transform.Translate(walkSpeed * Time.deltaTime * movementDir, camera.rotationYReference.transform);

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

        // Model animation
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
                animator.Play("Run");

            //model.rotation = Quaternion.LookRotation(movementDir);
        }
        else
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
                animator.GetCurrentAnimatorStateInfo(0).length * .9f <
             animator.GetCurrentAnimatorStateInfo(0).normalizedTime)
            {
                animator.Play("Idle");
            }
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
        //print("bye bye platform");
    }
}
