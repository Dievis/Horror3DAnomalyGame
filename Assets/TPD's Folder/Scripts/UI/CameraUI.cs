using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CameraUI : MonoBehaviourPunCallbacks
{
    public TMP_Text recText;          // Tham chiếu đến Text hiển thị chữ "REC"
    public Image circleImage;         // Tham chiếu đến Image hình tròn
    public float blinkSpeed = 0.5f;   // Tốc độ nhấp nháy (thay đổi nếu cần)

    public bool isRecording = false;  // Trạng thái quay video
    public GameObject cameraUIPanel;          // Tham chiếu đến Panel chứa các UI cần hiển thị/ẩn


    void Start()
    {
        // Đảm bảo panel được ẩn khi bắt đầu
        if (cameraUIPanel != null)
        {
            cameraUIPanel.SetActive(false);
        }

        // Đảm bảo video không quay khi bắt đầu
        isRecording = false;
        UpdateRecordingUI();
    }

    void Update()
    {
        // Kiểm tra nhấn phím F để bật/tắt panel và quay video
        if (Input.GetKeyDown(KeyCode.F) && PhotonNetwork.IsConnected)
        {
            TogglePanel();  // Mở hoặc đóng panel khi nhấn F
        }

        // Nếu đang quay
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

    // Hàm để bật/tắt panel và đồng thời điều khiển quay video
    public void TogglePanel()
    {
        if (cameraUIPanel != null)
        {
            // Nếu panel đang mở, tắt panel và dừng quay phim
            bool isPanelActive = cameraUIPanel.activeSelf;
            cameraUIPanel.SetActive(!isPanelActive);

            // Nếu panel đang được bật, bắt đầu quay
            if (!isPanelActive)
            {
                StartRecording();
            }
            else
            {
                StopRecording();
            }
        }
    }

    // Hàm bắt đầu quay video
    public void StartRecording()
    {
        if (photonView.IsMine)
        {
            isRecording = true;
            // Gửi RPC từ Player (hoặc đối tượng có PhotonView)
            photonView.RPC("UpdateRecordingState", RpcTarget.AllBuffered, isRecording);
        }
    }

    // Hàm dừng quay video
    public void StopRecording()
    {
        if (photonView.IsMine)
        {
            isRecording = false;
            // Gửi RPC để cập nhật trạng thái quay video trên tất cả client
            photonView.RPC("UpdateRecordingState", RpcTarget.AllBuffered, isRecording);
        }
    }

    // Phương thức RPC phải là public và không có giá trị trả về (void)
    [PunRPC]
    public void UpdateRecordingState(bool recordingState)
    {
        isRecording = recordingState;
        UpdateRecordingUI();  // Cập nhật UI khi trạng thái quay thay đổi
    }


    // Cập nhật UI dựa trên trạng thái quay video
    private void UpdateRecordingUI()
    {
        if (isRecording)
        {
            recText.text = "REC";
            float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1);
            circleImage.color = new Color(1, 0, 0, alpha);  // Đổi độ mờ của hình tròn
        }
        else
        {
            recText.text = "";  // Ẩn chữ "REC"
            circleImage.color = new Color(1, 0, 0, 0);  // Ẩn hình tròn (alpha = 0)
        }
    }
}
