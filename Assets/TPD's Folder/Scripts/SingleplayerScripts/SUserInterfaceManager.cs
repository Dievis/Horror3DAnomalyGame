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
    [SerializeField] private GameObject PlayerHUD;
    [SerializeField] private GameObject ExitButton;

    // Cập nhật số lượng anomaly trong UI
    public void UpdateAnomalyCountUI(int anomaliesFound, int totalAnomalies)
    {
        anomalyCountText.text = $"Anomaly: {anomaliesFound}/{totalAnomalies}";  // Hiển thị số lượng anomaly đã tìm thấy và tổng số anomaly
    }

    // Cập nhật thời gian trong UI
    public void UpdateTimerUI(float timer)
    {
        timerText.text = $"Time: {Mathf.CeilToInt(timer)}s";  // Hiển thị thời gian còn lại
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
