using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using DACNNEWMAP.PlayerControl;

public class UIManager : MonoBehaviourPunCallbacks
{
    [Header("UI Panels")] // Các panel UI
    [SerializeField] private GameObject winPanel; // Panel hiển thị khi người chơi chiến thắng
    [SerializeField] private GameObject losePanel; // Panel hiển thị khi người chơi thua
    [SerializeField] private GameObject countDown; // Panel đếm ngược khi bắt đầu game
    [SerializeField] private GameObject matchInfoPanel; // Panel hiển thị thông tin trận đấu
    [SerializeField] private TMP_Text anomalyCountText; // Text hiển thị số lượng anomaly đã tìm thấy
    [SerializeField] private TMP_Text timerText; // Text hiển thị thời gian còn lại
    [SerializeField] private TMP_Text explorationTimerText; // Text hiển thị thời gian khám phá
    [SerializeField] public TMP_Text anomalyScannedText; // Text hiển thị số anomaly đã scan
    [SerializeField] private GameObject hostLeftPanel; // Panel hiển thị khi chủ phòng rời đi

    [Header("Player Movement Settings")] // Cài đặt di chuyển cho người chơi
    [SerializeField] private MultiplayerController playerMovementScript;
    [SerializeField] private float originalWalkSpeed;
    [SerializeField] private float originalRunSpeed;
    private string walkSpeedFieldName = "_walkSpeed";  
    private string runSpeedFieldName = "_runSpeed";  

    [Header("Player Scripts")] // Các script liên quan đến người chơi
    [SerializeField] private SCameraUI cameraUIScript; // Script điều khiển camera UI

    private void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            // Tìm kiếm player của client hiện tại
            GameObject playerObject = PhotonNetwork.LocalPlayer.TagObject as GameObject;

            if (playerObject != null)
            {
                // Gán các script vào UIManager khi player spawn vào scene
                playerMovementScript = playerObject.GetComponent<MultiplayerController>();

                // Lưu lại tốc độ đi bộ và chạy gốc của người chơi
                if (playerMovementScript != null)
                {
                    originalWalkSpeed = playerMovementScript._walkSpeed;
                    originalRunSpeed = playerMovementScript._runSpeed;
                }
            }
        }
    }

    // Cập nhật số lượng anomaly trong giao diện người dùng
    public void UpdateAnomalyCountUI(int anomaliesFound, int totalAnomalies)
    {
        anomalyCountText.text = $"Vật thể bất thường: {anomaliesFound}/{totalAnomalies}";
    }

    // Cập nhật thời gian còn lại trong giao diện
    public void UpdateTimerUI(float timer)
    {
        timerText.text = $"Thời gian còn: {Mathf.CeilToInt(timer)}s";
    }

    // Cập nhật thời gian khám phá trong giao diện
    public void UpdateExplorationTimer(float remainingTime)
    {
        explorationTimerText.text = $"Bạn có {Mathf.CeilToInt(remainingTime)}s để khám phá.";
    }

    // Phương thức này sẽ được gọi từ NetworkManager
    public void ShowHostLeftPanel()
    {
        if (hostLeftPanel != null)
        {
            hostLeftPanel.SetActive(true); // Hiển thị panel host left
        }
    }

    // Ẩn explorationTimerText và hiển thị game timer
    public void HideExplorationTimerAndShowGameTimer()
    {
        explorationTimerText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(true);
    }

    // Hiển thị panel đếm ngược khi bắt đầu game
    public void ShowCountDownPanel()
    {
        if (countDown == null) return;
        countDown.SetActive(true);

        photonView.RPC("DisablePlayerMovement", RpcTarget.All); // Tắt di chuyển cho tất cả người chơi
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

        photonView.RPC("EnablePlayerMovement", RpcTarget.All); // Bật lại di chuyển cho tất cả người chơi
        HideCountDownPanel();
    }

    public void HideCountDownPanel()
    {
        if (countDown == null) return;
        countDown.SetActive(false);
    }

    // Hiển thị giao diện kết thúc trò chơi
    [PunRPC]
    public void ShowEndGameUI(bool victory)
    {
        if (victory)
        {
            winPanel.SetActive(true); // Nếu thắng, hiển thị panel chiến thắng
        }
        else
        {
            losePanel.SetActive(true); // Nếu thua, hiển thị panel thất bại
        }

        // Mở khóa con trỏ chuột khi trò chơi kết thúc
        UnlockCursor();
    }

    // Hiển thị hoặc ẩn panel thông tin trận đấu
    public void ToggleMatchInfoPanel()
    {
        matchInfoPanel.SetActive(!matchInfoPanel.activeSelf); // Đảo ngược trạng thái panel thông tin
    }

    // Reset lại giao diện khi bắt đầu trận đấu mới
    public void ResetUI()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        if (matchInfoPanel != null) matchInfoPanel.SetActive(false);
    }

    private void UnlockCursor()
    {
        // Mở khóa con trỏ chuột
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LockCursor()
    {
        // Khóa và ẩn con trỏ chuột
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    [PunRPC]
    private void DisablePlayerMovement()
    {
        if (playerMovementScript != null)
        {
            playerMovementScript.GetType().GetField(walkSpeedFieldName)?.SetValue(playerMovementScript, 0f);
            playerMovementScript.GetType().GetField(runSpeedFieldName)?.SetValue(playerMovementScript, 0f);
        }

        if (cameraUIScript != null) cameraUIScript.enabled = false;
    }

    [PunRPC]
    private void EnablePlayerMovement()
    {
        if (playerMovementScript != null)
        {
            playerMovementScript.GetType().GetField(walkSpeedFieldName)?.SetValue(playerMovementScript, originalWalkSpeed);
            playerMovementScript.GetType().GetField(runSpeedFieldName)?.SetValue(playerMovementScript, originalRunSpeed);
        }

        if (cameraUIScript != null) cameraUIScript.enabled = true;
    }
}
