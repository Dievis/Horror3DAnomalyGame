using UnityEngine;

public class HidingCameraController : MonoBehaviour
{
    public float sensitivity = 100f;
    public float upperLimit = -40f;
    public float lowerLimit = 40f;

    private float verticalRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // Xoay ngang 360 độ
        transform.parent.Rotate(Vector3.up * mouseX);

        // Giới hạn góc nhìn dọc
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, upperLimit, lowerLimit);

        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
