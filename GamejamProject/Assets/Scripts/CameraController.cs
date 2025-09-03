using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float sensitivity = 1;
    public Transform rotationYReference;
    void Start()
    {
        // Hide and lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Rotate camera anchor
        float newRotationY = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
        float newRotationX = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * sensitivity;
        this.transform.eulerAngles = new Vector3(newRotationX, newRotationY, 0);
        if (rotationYReference != null)
            rotationYReference.eulerAngles = new(0, this.transform.eulerAngles.y, 0);
    }
}
