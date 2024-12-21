using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SUserInterfaceManager : MonoBehaviour
{
    [Header("UI Panels")]  // Các panel UI
    [SerializeField] private GameObject winPanel;  // Panel hiển thị khi người chơi chiến thắng
    [SerializeField] private GameObject losePanel;  // Panel hiển thị khi người chơi thua
    [SerializeField] private GameObject countDown;  // Panel hướng dẫn
    [SerializeField] private GameObject matchInfoPanel;  // Panel thông tin trận đấu
    [SerializeField] private TMP_Text anomalyCountText;  // Text hiển thị số lượng anomaly đã tìm thấy
    [SerializeField] private TMP_Text timerText;  // Text hiển thị thời gian còn lại
    [SerializeField] private TMP_Text explorationTimerText; // Text hiển thị thời gian khám phá 
    [SerializeField] public TMP_Text AnomalyScannedText; // Text hiển thị thời gian khám phá 
    
    [SerializeField] private GameObject PlayerHUD;

    [Header("Player Movement Settings")]
    [SerializeField] private MonoBehaviour playerMovementScript; // Script di chuyển của Player
    [SerializeField] private float originalWalkSpeed; // Tốc độ đi bộ gốc của Player
    [SerializeField] private float originalRunSpeed;  // Tốc độ chạy gốc của Player
    [SerializeField] private string walkSpeedFieldName = "_walkSpeed"; // Tên trường tốc độ đi bộ
    [SerializeField] private string runSpeedFieldName = "_runSpeed";  // Tên trường tốc độ chạy

    [Header("Player Scripts")]
    [SerializeField] private SCameraUI cameraUIScript; // Script SCameraUI

    private void Awake()
    {
        if (playerMovementScript != null)
        {
            // Lưu giá trị tốc độ gốc khi khởi tạo
            originalWalkSpeed = (float)playerMovementScript.GetType().GetField(walkSpeedFieldName)?.GetValue(playerMovementScript);
            originalRunSpeed = (float)playerMovementScript.GetType().GetField(runSpeedFieldName)?.GetValue(playerMovementScript);
        }
    }

    // Cập nhật số lượng anomaly trong UI
    public void UpdateAnomalyCountUI(int anomaliesFound, int totalAnomalies)
    {
        anomalyCountText.text = $"Vật thể bất thường: {anomaliesFound}/{totalAnomalies}";  // Hiển thị số lượng anomaly đã tìm thấy và tổng số anomaly
    }

    // Cập nhật thời gian trong UI
    public void UpdateTimerUI(float timer)
    {
        timerText.text = $"Thời gian còn: {Mathf.CeilToInt(timer)}s";  // Hiển thị thời gian còn lại
    }

    // Cập nhật UI thời gian khám phá
    public void UpdateExplorationTimer(float remainingTime)
    {
        explorationTimerText.text = $"Bạn có {Mathf.CeilToInt(remainingTime)}s để khám phá.";  // Hiển thị thời gian khám phá
    }

    //Hàm tắt explorationTimerText sau khi hết thời gian bật timerText
    public void HideExplorationTimerAndShowGameTimer()
    {
        explorationTimerText.gameObject.SetActive(false);  // Ẩn exploration timer
        timerText.gameObject.SetActive(true);  // Hiển thị game timer
    }


    public void ShowCountDownPanel()
    {
        if (countDown == null) return;
        countDown.SetActive(true);
        if (PlayerHUD != null) PlayerHUD.SetActive(false);

        // Vô hiệu hóa Player di chuyển và SCameraUI
        if (playerMovementScript != null)
        {
            // Đặt tốc độ đi bộ và chạy về 0
            playerMovementScript.GetType().GetField(walkSpeedFieldName)?.SetValue(playerMovementScript, 0f);
            playerMovementScript.GetType().GetField(runSpeedFieldName)?.SetValue(playerMovementScript, 0f);
        }

        if (cameraUIScript != null) cameraUIScript.enabled = false;

        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        TMP_Text countdownText = countDown.GetComponentInChildren<TMP_Text>();
        if (countdownText == null) yield break;

        int countdownValue = 5;
        while (countdownValue > 0)
        {
            countdownText.text = countdownValue.ToString();
            yield return new WaitForSeconds(1f);
            countdownValue--;
        }

        // Khôi phục tốc độ di chuyển của Player và SCameraUI
        if (playerMovementScript != null)
        {
            playerMovementScript.GetType().GetField(walkSpeedFieldName)?.SetValue(playerMovementScript, originalWalkSpeed);
            playerMovementScript.GetType().GetField(runSpeedFieldName)?.SetValue(playerMovementScript, originalRunSpeed);
        }

        if (cameraUIScript != null) cameraUIScript.enabled = true;

        HideCountDownPanel();
    }

    public void HideCountDownPanel()
    {
        if (countDown == null) return;
        countDown.SetActive(false);
        if (PlayerHUD != null) PlayerHUD.SetActive(true);
    }

    // Hiển thị UI khi kết thúc trò chơi (thắng hoặc thua)
    public void ShowEndGameUI(bool victory)
    {
        if (victory)
        {
            winPanel.SetActive(true);  // Nếu thắng, hiển thị panel chiến thắng
        }
        else
        {
            losePanel.SetActive(true);  // Nếu thua, hiển thị panel thất bại
        }
    }

    // Hiển thị hoặc ẩn panel thông tin trận đấu
    public void ToggleMatchInfoPanel()
    {
        matchInfoPanel.SetActive(!matchInfoPanel.activeSelf);  // Đảo ngược trạng thái của panel thông tin trận đấu
    }



    // Reset lại tất cả UI về trạng thái ban đầu
    public void ResetUI()
    {
        if (winPanel != null) winPanel.SetActive(false);  // Tắt panel chiến thắng
        if (losePanel != null) losePanel.SetActive(false);  // Tắt panel thất bại
        if (matchInfoPanel != null) matchInfoPanel.SetActive(false);  // Tắt panel thông tin trận đấu
    }

    private void UnlockCursor()
    {
        // Mở khóa con trỏ khi cần thiết
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;  // Khóa con trỏ chuột
        Cursor.visible = false;  // Ẩn con trỏ chuột
    }

}
