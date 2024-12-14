using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    // UI Panels
    [Header("UI Panels")]
    [SerializeField] private GameObject hostLeftPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject gameInfoPanel;

    // Game Stats
    [Header("Game Stats")]
    [SerializeField] private int totalAnomalies;
    [SerializeField] private int anomaliesFound = 0;
    [SerializeField] private float gameDuration = 60f; 
    [SerializeField] private float timer;
    [SerializeField] private bool gameEnded = false;

    // UI Elements to display the anomaly count and time remaining
    [Header("UI Elements - Game Info")]
    [SerializeField] private TMP_Text anomalyCountText;  // Text UI to display anomaly count
    [SerializeField] private TMP_Text timerText;         // Text UI to display remaining time

    private void Start()
    {
        timer = gameDuration;
        totalAnomalies = GameObject.FindGameObjectsWithTag("Anomaly").Length;

        // Initialize the UI with the current anomaly count and time
        UpdateAnomalyCountUI();
        UpdateTimerUI();

        // Ensure game info panel is hidden at the start
        gameInfoPanel.SetActive(false);
    }

    private void UpdateAnomalyCountUI()
    {
        if (anomalyCountText != null)
        {
            anomalyCountText.text = $"Anomaly: {anomaliesFound}/{totalAnomalies}";
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = $"Thời gian: {Mathf.CeilToInt(timer)}s"; // Hiển thị thời gian còn lại
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} đã rời game.");
        RemovePlayerObjects(otherPlayer);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("Master Client đã rời đi. Đuổi tất cả người chơi ra khỏi phòng.");
        PhotonNetwork.LeaveRoom();
        Loader.Load(Loader.Scene.LoadingScene, Loader.Scene.MainMenuScene);
    }

    private void RemovePlayerObjects(Player player)
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView view = obj.GetComponent<PhotonView>();
            if (view != null && view.Owner == player)
            {
                PhotonNetwork.Destroy(obj);
            }
        }
    }

    private void ShowHostLeftPanel()
    {
        if (hostLeftPanel != null) hostLeftPanel.SetActive(true);
        StartCoroutine(ReturnToMenuAfterDelay());
    }

    private IEnumerator ReturnToMenuAfterDelay()
    {
        yield return new WaitForSeconds(3);

        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.LeaveRoom();
            Loader.Load(Loader.Scene.LoadingScene, Loader.Scene.MainMenuScene);
        }
        else
        {
            Debug.LogWarning("Photon client không còn kết nối khi đang rời phòng.");
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Mất kết nối khỏi Photon.");
        Loader.Load(Loader.Scene.LoadingScene, Loader.Scene.MainMenuScene);
    }

    private void Update()
    {
        if (gameEnded) return;

        timer -= Time.deltaTime;

        // Update the timer text in UI
        UpdateTimerUI();

        if (timer <= 0f)
        {
            EndGame(false); // Thua do hết thời gian
        }

        if (anomaliesFound >= totalAnomalies)
        {
            EndGame(true); // Thắng do tìm đủ anomaly
        }

        // Check for Tab key press to toggle the game info panel visibility
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleGameInfoPanel();
        }
    }

    private void ToggleGameInfoPanel()
    {
        // Toggle visibility of the game info panel
        gameInfoPanel.SetActive(!gameInfoPanel.activeSelf);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    [PunRPC]
    public void OnAnomalyFound()
    {
        anomaliesFound++;
        UpdateAnomalyCountUI();  // Cập nhật UI mỗi khi tìm thấy anomaly
    }

    private void EndGame(bool victory)
    {
        gameEnded = true;

        UnlockCursor();

        if (victory)
        {
            winPanel.gameObject.SetActive(true);
            Debug.Log("Người chơi đã chiến thắng!");
            // Hiển thị UI chiến thắng
        }
        else
        {
            Debug.Log("Người chơi đã thua cuộc!");
            losePanel.gameObject.SetActive(true);
            // Hiển thị UI thất bại
        }

        StartCoroutine(ReturnToMenuAfterDelay());
    }

    public void NotifyAnomalyFound()
    {
        photonView.RPC("OnAnomalyFound", RpcTarget.All);
    }
}