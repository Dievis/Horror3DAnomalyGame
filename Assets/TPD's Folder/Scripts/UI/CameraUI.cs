using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraUI : MonoBehaviour
{
    public TMP_Text recText;          // Tham chiếu đến Text hiển thị chữ "REC"
    public Image circleImage;     // Tham chiếu đến Image hình tròn
    public float blinkSpeed = 0.5f;  // Tốc độ nhấp nháy (thay đổi nếu cần)

    private bool isRecording = true;  // Trạng thái quay video

    void Update()
    {
        if (isRecording)
        {
            // Làm cho hình tròn nhấp nháy
            float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1);
            circleImage.color = new Color(1, 0, 0, alpha);  // Đổi độ mờ của hình tròn

            // Cập nhật chữ REC
            recText.text = "REC";
        }
        else
        {
            // Ẩn hình tròn khi không quay
            circleImage.color = new Color(1, 0, 0, 0);  // Ẩn hình tròn (alpha = 0)
            recText.text = "";  // Ẩn chữ "REC"
        }
    }

    // Hàm để bắt đầu/quay video
    public void StartRecording()
    {
        isRecording = true;
    }

    // Hàm để dừng quay video
    public void StopRecording()
    {
        isRecording = false;
    }
}
