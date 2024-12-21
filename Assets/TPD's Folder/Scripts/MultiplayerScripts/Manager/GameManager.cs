using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Game Stats")]
    [SerializeField] private int totalAnomalies;        // Tổng số anomalies trong game
    [SerializeField] public int anomaliesFound = 0;     // Số anomalies đã tìm thấy
    [SerializeField][Range(0f, 1f)] private float victoryThreshold = 0.8f; // Tỷ lệ chiến thắng (phần trăm anomalies cần tìm để thắng)
    private bool gameEnded = false;   // Trạng thái game (đã kết thúc hay chưa)

    public float gameDuration = 180f;   // Thời gian của game
    public float timer;               // Bộ đếm thời gian

    // Singleton instance (dùng để quản lý đối tượng game manager)
    public static GameManager instance;

    private CameraUI cameraUI;
    private UIManager uiManager;  // Quản lý giao diện người dùng
    public HashSet<GameObject> anomaliesProcessed = new HashSet<GameObject>();  // Tập hợp các anomalies đã được xử lý (tìm thấy hoặc đã xóa)

    public AudioSource GameOver;

    public AudioSource Win;

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
        cameraUI = FindObjectOfType<CameraUI>();
        uiManager = FindObjectOfType<UIManager>();  // Tìm và gán SUserInterfaceManager
        timer = gameDuration;  // Khởi tạo thời gian

        // Chỉ hiển thị Panel Hướng dẫn nếu chưa từng hiển thị
        uiManager.ShowCountDownPanel();

        // Reset UI ngay lập tức để tránh hiển thị sai thông tin
        uiManager.ResetUI();

        // Cập nhật UI ban đầu
        uiManager.UpdateAnomalyCountUI(anomaliesFound, totalAnomalies);

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
            uiManager.UpdateAnomalyCountUI(anomaliesFound, totalAnomalies);

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

    // Phương thức thông báo khi tìm thấy anomaly
    [PunRPC]
    public void NotifyAnomalyFound(int anomalyViewID)
    {
        GameObject anomaly = PhotonView.Find(anomalyViewID)?.gameObject;  // Retrieve anomaly from PhotonView ID
        if (anomaly != null && !anomaliesProcessed.Contains(anomaly))
        {
            anomaliesProcessed.Add(anomaly);
            anomaliesFound++;  // Tăng số anomaly đã tìm

            Debug.Log("Anomaly found! Total anomalies found: " + anomaliesFound);

            // Tăng thời gian game mỗi khi tìm thấy anomaly
            timer += 10f;

            // Kiểm tra và phục hồi pin mỗi khi tìm thấy 5 anomalies
            if (anomaliesFound % 5 == 0)
            {
                RestoreBatteryLevel(1); // Restores 1 level of battery
            }

            cameraUI.timePassed -= 5; // Điều chỉnh thời gian đã trôi qua cho việc cạn pin

            // Cập nhật UI với số anomalies đã tìm và thời gian còn lại
            uiManager.UpdateAnomalyCountUI(anomaliesFound, totalAnomalies - anomaliesProcessed.Count);
            uiManager.UpdateTimerUI(timer);

            // Kiểm tra điều kiện kết thúc game
            CheckGameOver();
        }
        else
        {
            Debug.LogWarning("Anomaly not found.");
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

    // Kiểm tra điều kiện kết thúc game
    private void CheckGameOver()
    {
        float requiredAnomalies = totalAnomalies * victoryThreshold;  // Tính toán số anomalies cần thiết để thắng
        if (anomaliesFound >= requiredAnomalies)
        {
            // Phát âm thanh khi thắng
            if (Win != null)
            {
                Win.Play();
            }
            UnlockCursor(); // Mở khóa trỏ chuột để tương tác khi chiến thắng
            EndGame(true);  // Chiến thắng nếu số anomalies cần tìm đã đủ
        }
        else if (timer <= 0f)
        {
            // Phát âm thanh khi thua
            if (GameOver != null)
            {
                GameOver.Play();
            }
            UnlockCursor();  // Mở khóa trỏ chuột để tương tác khi thua
            EndGame(false);  // Thua nếu hết thời gian
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

    // Phương thức reset lại game
    public void ResetGame()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        gameEnded = false;
        anomaliesFound = 0;
        timer = gameDuration;
        anomaliesProcessed.Clear();

        uiManager.UpdateAnomalyCountUI(anomaliesFound, totalAnomalies);
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
