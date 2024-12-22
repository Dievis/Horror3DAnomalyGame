using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun.Demo.PunBasics;

public class SCameraUI : MonoBehaviour
{
    public TMP_Text recText;          // Tham chiếu đến Text hiển thị chữ "REC"
    public Image circleImage;         // Tham chiếu đến Image hình tròn
    public float blinkSpeed = 0.5f;   // Tốc độ nhấp nháy (thay đổi nếu cần)

    public bool isRecording = false;  // Trạng thái quay video
    public GameObject cameraUIPanel;  // Tham chiếu đến Panel chứa các UI cần hiển thị/ẩn
    public GameObject FlashLight;

    public Image BatteryImg;          // Tham chiếu đến Image hiển thị pin
    public Sprite FullBattery;        // Sprite cho pin đầy
    public Sprite HalfBattery;        // Sprite cho pin một nửa
    public Sprite ChargeBattery;      // Sprite cho pin cần sạc

    [Header("Battery level: 3 = Full, 2 = Half, 1 = Charge, 0 = Game over")]
    public int batteryLevel = 3;     // Mức pin: 3 là đầy, 2 là một nửa, 1 là cần sạc, 0 là thua
    private Coroutine blinkCoroutine;
    [Header("Time passed >= 30 : -1 battery level")]// Tham chiếu đến Coroutine nhấp nháy
    public float timePassed = 0f;    // Biến đếm thời gian trôi qua mỗi frame

    private SGameManager gameManager;

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
        // Cập nhật mức pin và giảm nếu đang quay video
        UpdateBatteryLevel();

        // Kiểm tra nhấn phím F để bật/tắt panel và quay video
        if (Input.GetKeyDown(KeyCode.F))
        {
            TogglePanel();  // Bật/ tắt panel hiển thị camera
        }
    }

    // Cập nhật UI khi quay video
    public void UpdateRecordingUI()
    {
        if (isRecording)
        {
            // Nếu đang quay, bật màu đỏ cho REC
            recText.text = "REC";
            circleImage.color = Color.red;

            // Bắt đầu nhấp nháy màu đỏ cho circleImage
            StartBlinking();
        }
        else
        {
            // Nếu không quay, ẩn REC và hình tròn
            recText.text = "";
            circleImage.color = Color.clear;

            // Dừng nhấp nháy khi dừng quay video
            StopBlinking();
        }
    }

    // Hàm bắt đầu quay
    public void StartRecording()
    {
        isRecording = true;
        FlashLight.SetActive(true);
        Debug.Log("Recording Started");
        UpdateRecordingUI();
    }

    // Hàm dừng quay
    public void StopRecording()
    {
        isRecording = false;
        FlashLight.SetActive(false);
        Debug.Log("Recording Stopped");
        UpdateRecordingUI();
    }

    // Cập nhật mức độ pin và giảm nếu đang quay
    private void UpdateBatteryLevel()
    {
        if (isRecording)
        {
            // Mỗi 60 giây, giảm một cấp pin
            timePassed += Time.deltaTime;  // Tăng thời gian đã trôi qua mỗi frame

            if (timePassed >= 30f)  // Mỗi 30 giây
            {
                timePassed = 0f;  // Reset bộ đếm thời gian
                DecreaseBatteryLevel();  // Giảm pin một mức
            }
        }

        UpdateBatteryUI();
    }

    // Giảm pin một cấp
    private void DecreaseBatteryLevel()
    {
        if (batteryLevel > 0)
        {
            batteryLevel--;
            Debug.Log("Battery Level: " + batteryLevel);
        }

        // Nếu pin hết, dừng quay video
        if (batteryLevel == 0)
        {
            StopRecording();
        }
    }

    // Cập nhật UI cho pin
    public void UpdateBatteryUI()
    {
        if (batteryLevel == 3)
        {
            BatteryImg.sprite = FullBattery;
        }
        else if (batteryLevel == 2)
        {
            BatteryImg.sprite = HalfBattery;
        }
        else if (batteryLevel == 1)
        {
            BatteryImg.sprite = ChargeBattery;
        }
        else
        {
            gameManager.EndGame(false);
            Debug.LogWarning("Battery is dead!");
        }
    }

    // Toggle panel hiển thị camera
    public void TogglePanel()
    {
        if (cameraUIPanel != null)
        {
            bool isActive = cameraUIPanel.activeSelf;
            cameraUIPanel.SetActive(!isActive);  // Nếu panel đang hiển thị thì ẩn đi, ngược lại thì hiện lên

            // Kiểm tra trạng thái của panel và thay đổi trạng thái quay video
            if (cameraUIPanel.activeSelf)  // Panel được bật lên (hiển thị)
            {
                StartRecording();  // Bắt đầu quay video khi panel hiển thị
            }
            else  // Panel bị tắt (ẩn đi)
            {
                StopRecording();  // Dừng quay video khi panel ẩn đi
            }
        }
    }

    // Coroutine để tạo hiệu ứng nhấp nháy màu đỏ cho circleImage
    private IEnumerator BlinkCircle()
    {
        while (isRecording)
        {
            circleImage.color = Color.red;  // Màu đỏ
            yield return new WaitForSeconds(blinkSpeed);
            circleImage.color = new Color(1f, 0f, 0f, 0f); // Màu trong suốt
            yield return new WaitForSeconds(blinkSpeed);
        }
    }

    // Bắt đầu nhấp nháy màu đỏ cho circleImage
    private void StartBlinking()
    {
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);  // Dừng coroutine nếu đang chạy
        blinkCoroutine = StartCoroutine(BlinkCircle());
    }

    // Dừng nhấp nháy
    private void StopBlinking()
    {
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);  // Dừng coroutine khi không cần nhấp nháy nữa
        circleImage.color = Color.clear;  // Đảm bảo circleImage ẩn đi khi không quay
    }
}
