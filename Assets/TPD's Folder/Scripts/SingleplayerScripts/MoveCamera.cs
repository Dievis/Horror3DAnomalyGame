using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float mouseSensitivity = 15f; // Độ nhạy của chuột

    private float xRotation = 0f; // Biến lưu góc quay theo trục X
    private float yRotation = 0f; // Biến lưu góc quay theo trục Y

    void Start()
    {
        // Khóa con trỏ chuột ở giữa màn hình và ẩn con trỏ
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Lấy giá trị di chuyển của chuột
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Điều chỉnh góc quay trục X (lên/xuống) và giới hạn trong khoảng -90 đến 90 độ
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Điều chỉnh góc quay trục Y (trái/phải)
        yRotation += mouseX;

        // Gán góc quay cho transform
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
