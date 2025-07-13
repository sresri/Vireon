using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float mouseSensitivity = 2f;

    float pitch = 0f;
    float yaw = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        float moveY = 0f;

        if (Input.GetKey(KeyCode.Space)) moveY += 1f;
        if (Input.GetKey(KeyCode.Tab)) moveY -= 1f;

        Vector3 move = new Vector3(moveX, moveY, moveZ);
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.Self);

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}