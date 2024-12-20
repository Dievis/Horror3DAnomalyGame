using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SUserInterfaceManager : MonoBehaviour
{
    [Header("UI Panels")]  // Các panel UI
    [SerializeField] private GameObject winPanel;  // Panel hiển thị khi người chơi chiến thắng
    [SerializeField] private GameObject losePanel;  // Panel hiển thị khi người chơi thua
    [SerializeField] private GameObject tutorialPanel;  // Panel hướng dẫn
    [SerializeField] private GameObject matchInfoPanel;  // Panel thông tin trận đấu
    [SerializeField] private TMP_Text anomalyCountText;  // Text hiển thị số lượng anomaly đã tìm thấy
    [SerializeField] private TMP_Text timerText;  // Text hiển thị thời gian còn lại
    [SerializeField] private TMP_Text explorationTimerText; // Text hiển thị thời gian khám phá 
    [SerializeField] private GameObject PlayerHUD;
    [SerializeField] private GameObject ExitButton;

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


    // Hiển thị panel hướng dẫn
    public void ShowTutorialPanel()
    {
        if (tutorialPanel == null) return;
        tutorialPanel.SetActive(true); // Kích hoạt panel hướng dẫn
        if (PlayerHUD != null) PlayerHUD.SetActive(false); // Tắt HUD để tránh xung đột
        UnlockCursor();

    }

    // Ẩn panel hướng dẫn
    public void HideTutorialPanel()
    {
        if (tutorialPanel == null) return;
        tutorialPanel.SetActive(false);  // Tắt panel hướng dẫn
        if (PlayerHUD != null) PlayerHUD.SetActive(true); // Bật lại HUD
    }

    public void ShowExitButton()
    {
        if (ExitButton == null) return;
        ExitButton.SetActive(true);  // Hiển thị nút ExitButton
    }

    // Thêm phương thức xử lý ExitButton
    public void OnExitButtonClick()
    {
        HideTutorialPanel();  // Đảm bảo ẩn đúng tutorial panel
        LockCursor();
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
