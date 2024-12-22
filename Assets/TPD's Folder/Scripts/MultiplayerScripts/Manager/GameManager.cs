using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Game Stats")]    
    public int totalAnomalies = 0; // Tổng số anomalies trong game
    public HashSet<int> anomaliesProcessed = new HashSet<int>();
    [SerializeField][Range(0f, 1f)] private float victoryThreshold = 0.8f; // Tỷ lệ chiến thắng (phần trăm anomalies cần tìm để thắng)
    private bool gameEnded = false;   // Trạng thái game (đã kết thúc hay chưa)

    public float gameDuration = 180f;   // Thời gian của game
    public float timer;               // Bộ đếm thời gian

    // Singleton instance (dùng để quản lý đối tượng game manager)
    public static GameManager instance;

    public CameraUI cameraUI;
    public UIManager uiManager;  // Quản lý giao diện người dùng



    public AudioSource GameOver;

    public AudioSource Win;

    private int playersOutOfBattery = 0;
    private int totalPlayers = 0;

    private void Awake()
    {
        // Singleton pattern (đảm bảo chỉ có một instance của SGameManager)
        if (instance != null && instance != this)
        {
            Destroy(gameObject);  // Nếu đã có một instance khác, hủy đối tượng này
            return;
        }
        instance = this;  // Gán instance hiện tại
    }

    private void Start()
    {
        totalPlayers = PhotonNetwork.PlayerList.Length;
        cameraUI = FindObjectOfType<CameraUI>();
        uiManager = FindObjectOfType<UIManager>();  // Tìm và gán SUserInterfaceManager
        timer = gameDuration;  // Khởi tạo thời gian

        // Chỉ hiển thị Panel Hướng dẫn nếu chưa từng hiển thị
        uiManager.ShowCountDownPanel();

        // Reset UI ngay lập tức để tránh hiển thị sai thông tin
        uiManager.ResetUI();

        // Tìm tất cả anomaly trong scene và đếm tổng số anomalies
        totalAnomalies = GameObject.FindGameObjectsWithTag("Anomaly").Length;

        // Cập nhật UI ban đầu
        uiManager.UpdateAnomalyCountUI(anomaliesProcessed.Count, totalAnomalies);

        // Bắt đầu game và spawn anomalies
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(WaitForAnomaliesToSpawn());
        }
    }

    // Coroutine để đợi spawn anomalies
    private IEnumerator WaitForAnomaliesToSpawn()
    {
        // Đợi một khoảng thời gian trước khi spawn anomalies
        float waitTime = 5f; // Ví dụ, đợi 5 giây
        yield return new WaitForSeconds(waitTime);

        // Kiểm tra tổng số anomalies sau khi đợi
        totalAnomalies = GameObject.FindGameObjectsWithTag("Anomaly").Length;

        if (totalAnomalies > 0)
        {
            Debug.Log("Trò chơi đã bắt đầu! Tổng anomalies: " + totalAnomalies);
            uiManager.UpdateAnomalyCountUI(anomaliesProcessed.Count, totalAnomalies);

            // Gọi HideCountDownPanel khi tất cả anomalies đã spawn xong
            uiManager.HideCountDownPanel();

            yield return StartCoroutine(Wait1Min());
        }
        else
        {
            Debug.LogWarning("Không có vật thể bất thường sau khi chờ đợi. Trò chơi sẽ không bắt đầu.");
            // Nếu không có anomalies, kết thúc game ngay lập tức
            EndGame(false);
        }
    }

    private IEnumerator Wait1Min()
    {
        float waitTime = 60f; // 1 phút (60 giây)
        float elapsedTime = 0f;

        // Hiển thị thông báo cho người chơi
        uiManager.UpdateExplorationTimer(waitTime);  // Hiển thị UI thông báo về thời gian khám phá

        // Chờ đợi 1 phút sau khi người chơi tắt TutorialPanel
        while (elapsedTime < waitTime)
        {
            elapsedTime += Time.deltaTime;
            uiManager.UpdateExplorationTimer(waitTime - elapsedTime);  // Cập nhật lại thời gian khám phá trên UI
            yield return null;  // Chờ đến frame tiếp theo
        }

        // Sau 1 phút, game sẽ bắt đầu
        Debug.Log("1p khám phá đã hết, bắt đầu đếm ngược thời gian.");

        // Gọi phương thức ẩn exploration timer và hiển thị game timer
        uiManager.HideExplorationTimerAndShowGameTimer();

        StartGameTimer();  // Bắt đầu đếm thời gian game
    }

    // Phương thức bắt đầu đếm thời gian game
    private void StartGameTimer()
    {
        // Bắt đầu đếm thời gian
        uiManager.UpdateTimerUI(timer);
        StartCoroutine(TimerCoroutine());
    }

    // Coroutine để đếm thời gian
    private IEnumerator TimerCoroutine()
    {
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            uiManager.UpdateTimerUI(timer);  // Cập nhật UI timer
            yield return null;  // Chờ đến frame tiếp theo
        }
        // Kết thúc game khi hết thời gian
        EndGame(false);
    }

    private void Update()
    {
        if (gameEnded) return;

        // Kết thúc game nếu hết thời gian
        if (timer <= 0f) EndGame(false);

        // Chuyển đổi hiển thị thông tin trận đấu khi nhấn Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            uiManager.ToggleMatchInfoPanel();
        }
    }

    public void CheckAllPlayersOutOfBattery()
    {
        playersOutOfBattery++;

        // Nếu tất cả người chơi đều hết pin, gọi hàm kết thúc game
        if (playersOutOfBattery >= totalPlayers)
        {
            EndGame(false);  // Thua game
        }
    }

    // Phương thức thông báo khi tìm thấy anomaly
    [PunRPC]
    public void NotifyAnomalyFound(int anomalyViewID)
    {
        Debug.Log("Received NotifyAnomalyFound RPC with ViewID: " + anomalyViewID);
        GameObject anomaly = PhotonView.Find(anomalyViewID)?.gameObject;  // Retrieve anomaly from PhotonView ID

        if (anomaly != null)
        {
            // Tăng số anomalies đã xử lý
            anomaliesProcessed.Add(anomalyViewID);  // Thêm anomaly vào HashSet

            // Chờ đến khi anomaly thực sự được xóa
            photonView.RPC("ConfirmAnomalyDestruction", RpcTarget.All, anomalyViewID);
        }
        else
        {
            Debug.LogWarning("Anomaly not found for ViewID: " + anomalyViewID);
        }
    }

    // RPC để xác nhận xóa anomaly và cập nhật anomaliesFound
    [PunRPC]
    public void ConfirmAnomalyDestruction(int anomalyViewID)
    {
        GameObject anomaly = PhotonView.Find(anomalyViewID)?.gameObject;

        if (anomaly != null)
        {
            // Đảm bảo rằng anomaly đã được xử lý và không bị lặp lại
            Debug.Log("Anomaly found! Total anomalies found: " + anomaliesProcessed);

            // Tăng thời gian game mỗi khi tìm thấy anomaly
            timer += 20f;

            // Kiểm tra và phục hồi pin mỗi khi tìm thấy 3 anomalies
            if (anomaliesProcessed.Count % 3 == 0)
            {
                RestoreBatteryLevel(1); // Restores 1 level of battery
            }

            cameraUI.timePassed -= 5; // Điều chỉnh thời gian đã trôi qua cho việc cạn pin

            // Cập nhật UI với số anomalies đã tìm và thời gian còn lại
            uiManager.UpdateAnomalyCountUI(anomaliesProcessed.Count, totalAnomalies);
            uiManager.UpdateTimerUI(timer);

            // Kiểm tra điều kiện kết thúc game
        }
    }

    // Phương thức phục hồi pin
    private void RestoreBatteryLevel(int amount)
    {
        // Nếu mức pin chưa đạt max (3), cộng thêm amount vào mức pin
        if (cameraUI.batteryLevel < 3)
        {
            cameraUI.batteryLevel += amount;

            // Đảm bảo mức pin không vượt quá giới hạn (max là 3)
            cameraUI.batteryLevel = Mathf.Min(cameraUI.batteryLevel, 3);

            // Cập nhật UI mức pin
            cameraUI.UpdateBatteryUI();
        }
    }

    public void CheckVictoryCondition()
    {
        if (anomaliesProcessed.Count == totalAnomalies)
        {
            EndGame(true); // Victory
        }
        else if (timer <= 0f)
        {
            EndGame(false); // Defeat
        }
    }


    // Phương thức kết thúc game
    public void EndGame(bool victory)
    {
        if (gameEnded) return;

        gameEnded = true;  // Đánh dấu game đã kết thúc
        ShowEndGameUI(victory);  // Hiển thị UI kết thúc game
    }

    // Hiển thị UI kết thúc game (thắng hay thua)
    public void ShowEndGameUI(bool victory)
    {
        // Gửi RPC thông báo kết thúc game cho tất cả các client
        photonView.RPC("ShowEndGameUI_RPC", RpcTarget.All, victory);
    }

    // RPC để hiển thị UI kết thúc game trên tất cả client
    [PunRPC]
    public void ShowEndGameUI_RPC(bool victory)
    {
        uiManager.ShowEndGameUI(victory); // Gọi phương thức để hiển thị kết quả thắng/thua
    }

    public void ResetGame()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        gameEnded = false;
        anomaliesProcessed.Clear(); 
        timer = gameDuration;

        uiManager.UpdateAnomalyCountUI(anomaliesProcessed.Count, totalAnomalies);
        uiManager.UpdateTimerUI(timer);

        Debug.Log("Game has been reset.");

        // Spawn lại anomalies nếu là MasterClient
        StartCoroutine(WaitForAnomaliesToSpawn());
    }


    // Phương thức quay lại menu chính khi bấm nút
    public void OnClickBackToMenu()
    {
        Debug.Log("Quay lại menu chính.");
        if (uiManager != null)
        {
            uiManager.ResetUI();
        }
        PhotonNetwork.LeaveRoom(); // Thoát khỏi phòng
        Loader.Load(Loader.Scene.LoadingScene, Loader.Scene.MainMenuScene); // Load scene menu chính
    }

    private void UnlockCursor()
    {
        // Mở khóa con trỏ khi cần thiết
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
