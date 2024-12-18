using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SCameraUI : MonoBehaviour
{
    public TMP_Text recText;          // Tham chiếu đến Text hiển thị chữ "REC"
    public Image circleImage;         // Tham chiếu đến Image hình tròn
    public float blinkSpeed = 0.5f;   // Tốc độ nhấp nháy (thay đổi nếu cần)

    public bool isRecording = false;  // Trạng thái quay video
    public GameObject cameraUIPanel;  // Tham chiếu đến Panel chứa các UI cần hiển thị/ẩn

    public Image BatteryImg;          // Tham chiếu đến Image hiển thị pin
    public Sprite FullBattery;        // Sprite cho pin đầy
    public Sprite HalfBattery;        // Sprite cho pin một nửa
    public Sprite ChargeBattery;      // Sprite cho pin cần sạc

    private SGameManager gameManager; // Tham chiếu đến SGameManager
    private int batteryLevel = 3;     // Mức pin: 3 là đầy, 2 là một nửa, 1 là cần sạc, 0 là thua

    void Start()
    {
        // Lấy tham chiếu đến SGameManager
        gameManager = SGameManager.instance;

        if (gameManager == null)
        {
            Debug.LogError("SGameManager instance is missing in the scene.");
        }

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
        // Cập nhật mức pin dựa trên thời gian từ SGameManager
        UpdateBatteryLevel();

        // Kiểm tra nhấn phím F để bật/tắt panel và quay video
        if (Input.GetKeyDown(KeyCode.F))
        {
            TogglePanel();  // Mở hoặc đóng panel khi nhấn F
        }

        // Nếu đang quay
        if (isRecording)
        {
            // Làm cho hình tròn nhấp nháy
            float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1);
            circleImage.color = new Color(1, 0, 0, alpha);  // Đổi độ mờ của hình tròn
            recText.text = "REC";
        }
        else
        {
            // Ẩn hình tròn khi không quay
            circleImage.color = new Color(1, 0, 0, 0);  // Ẩn hình tròn (alpha = 0)
            recText.text = "";  // Ẩn chữ "REC"
        }
    }

    public void TogglePanel()
    {
        if (cameraUIPanel != null)
        {
            bool isPanelActive = cameraUIPanel.activeSelf;
            cameraUIPanel.SetActive(!isPanelActive);

            Debug.Log("Camera UI Panel Active: " + cameraUIPanel.activeSelf);

            // Nếu mở panel, bắt đầu ghi hình
            if (!isPanelActive)
            {
                StartRecording();
            }
            else
            {
                StopRecording();
            }

            // Đảm bảo UI được cập nhật ngay lập tức
            UpdateRecordingUI();
        }
        else
        {
            Debug.LogError("cameraUIPanel is not assigned in the Inspector.");
        }
    }

    public void StartRecording()
    {
        isRecording = true;
        Debug.Log("Recording Started");
        UpdateRecordingUI();
    }

    public void StopRecording()
    {
        isRecording = false;
        Debug.Log("Recording Stopped");
        UpdateRecordingUI();
    }

    private void UpdateRecordingUI()
    {
        if (isRecording)
        {
            recText.text = "REC";
            float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1);
            circleImage.color = new Color(1, 0, 0, alpha);
            Debug.Log("UI Updated: Recording");
        }
        else
        {
            recText.text = "";
            circleImage.color = new Color(1, 0, 0, 0);
            Debug.Log("UI Updated: Not Recording");
        }
    }

    private void UpdateBatteryLevel()
    {
        if (gameManager == null) return;

        // Lấy tỷ lệ thời gian còn lại từ SGameManager
        float timeFraction = gameManager.timer / gameManager.gameDuration;

        // Xác định mức pin dựa trên tỷ lệ thời gian
        if (timeFraction <= 0f)
        {
            batteryLevel = 0; // Hết pin
        }
        else if (timeFraction <= 0.33f)
        {
            batteryLevel = 1; // Cần sạc
        }
        else if (timeFraction <= 0.67f)
        {
            batteryLevel = 2; // Pin nửa
        }
        else
        {
            batteryLevel = 3; // Pin đầy
        }

        // Cập nhật hình ảnh pin
        switch (batteryLevel)
        {
            case 3:
                BatteryImg.sprite = FullBattery;
                break;
            case 2:
                BatteryImg.sprite = HalfBattery;
                break;
            case 1:
                BatteryImg.sprite = ChargeBattery;
                break;
            case 0:
                Debug.Log("Game Over - Out of Battery!");
                gameManager.EndGame(false);
                break;
        }
    }
}
