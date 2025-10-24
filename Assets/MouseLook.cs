using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 2000f;

    public Transform playerBody;  // Assign the Player Capsule here

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Hide + lock cursor to screen center
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        // float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // xRotation -= mouseY;
        // xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Clamp vertical look

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // Look up/down
        playerBody.Rotate(Vector3.up * mouseX); // Turn left/right
    }
}