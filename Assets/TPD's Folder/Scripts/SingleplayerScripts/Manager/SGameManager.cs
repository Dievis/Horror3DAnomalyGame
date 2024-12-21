using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class SGameManager : MonoBehaviour
{
    [Header("Game Stats")]
    [SerializeField] private int totalAnomalies;        // Tổng số anomalies trong game
    [SerializeField] private int anomaliesFound = 0;     // Số anomalies đã tìm thấy
    [SerializeField][Range(0f, 1f)] private float victoryThreshold = 0.8f; // Tỷ lệ chiến thắng (phần trăm anomalies cần tìm để thắng)
    private bool gameEnded = false;   // Trạng thái game (đã kết thúc hay chưa)

    public float gameDuration = 180f;   // Thời gian của game
    public float timer;               // Bộ đếm thời gian

    // Singleton instance (dùng để quản lý đối tượng game manager)
    public static SGameManager instance;

    private SCameraUI scameraUI;
    private SUserInterfaceManager SUIManager;  // Quản lý giao diện người dùng
    private HashSet<GameObject> anomaliesProcessed = new HashSet<GameObject>();  // Tập hợp các anomalies đã được xử lý (tìm thấy hoặc đã xóa)

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
        scameraUI = FindObjectOfType<SCameraUI>();
        SUIManager = FindObjectOfType<SUserInterfaceManager>();  // Tìm và gán SUserInterfaceManager
        timer = gameDuration;  // Khởi tạo thời gian

        // Chỉ hiển thị Panel Hướng dẫn nếu chưa từng hiển thị
        SUIManager.ShowCountDownPanel();

        // Reset UI ngay lập tức để tránh hiển thị sai thông tin
        SUIManager.ResetUI();

        // Cập nhật UI ban đầu
        SUIManager.UpdateAnomalyCountUI(anomaliesFound, totalAnomalies);

        // Bắt đầu game và spawn anomalies
        StartCoroutine(WaitForAnomaliesToSpawn());
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
            SUIManager.UpdateAnomalyCountUI(anomaliesFound, totalAnomalies);

            // Gọi HideCountDownPanel khi tất cả anomalies đã spawn xong
            SUIManager.HideCountDownPanel();

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
        SUIManager.UpdateExplorationTimer(waitTime);  // Hiển thị UI thông báo về thời gian khám phá

        // Chờ đợi 1 phút sau khi người chơi tắt TutorialPanel
        while (elapsedTime < waitTime)
        {
            elapsedTime += Time.deltaTime;
            SUIManager.UpdateExplorationTimer(waitTime - elapsedTime);  // Cập nhật lại thời gian khám phá trên UI
            yield return null;  // Chờ đến frame tiếp theo
        }

        // Sau 1 phút, game sẽ bắt đầu
        Debug.Log("1p khám phá đã hết, bắt đầu đếm ngược thời gian.");

        // Gọi phương thức ẩn exploration timer và hiển thị game timer
        SUIManager.HideExplorationTimerAndShowGameTimer();

        StartGameTimer();  // Bắt đầu đếm thời gian game
    }

    // Phương thức bắt đầu đếm thời gian game
    private void StartGameTimer()
    {
        // Bắt đầu đếm thời gian
        SUIManager.UpdateTimerUI(timer);
        StartCoroutine(TimerCoroutine());
    }

    // Coroutine để đếm thời gian
    private IEnumerator TimerCoroutine()
    {
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            SUIManager.UpdateTimerUI(timer);  // Cập nhật UI timer
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
            SUIManager.ToggleMatchInfoPanel();
        }
    }

    // Phương thức thông báo khi tìm thấy anomaly
    public void NotifyAnomalyFound(GameObject anomaly)
    {
        if (anomaly != null && !anomaliesProcessed.Contains(anomaly))
        {
            anomaliesProcessed.Add(anomaly);  // Thêm anomaly vào danh sách đã xử lý
            anomaliesFound++;  // Tăng số lượng anomalies đã tìm thấy
            Debug.Log("Anomaly found! Total anomalies found: " + anomaliesFound);

            // Tăng thời gian gameDuration thêm 10 giây
            timer += 10f;

            // Kiểm tra và hồi phục pin nếu đã tìm thấy đủ 5 anomalies
            if (anomaliesFound % 5 == 0)  // Mỗi 5 anomalies sẽ hồi 1 cấp pin
            {
                RestoreBatteryLevel(1);  // Phục hồi 1 cấp pin
            }

            // Vì mỗi 60s pin sẽ tụt 1 cấp, nên khi tìm thấy anomaly, cộng lại thời gian này
            scameraUI.timePassed -= 5; // Điều chỉnh lại thời gian tụt pin để không bị tụt quá nhanh

            // Cập nhật UI số lượng anomalies đã tìm thấy
            SUIManager.UpdateAnomalyCountUI(anomaliesFound, totalAnomalies - anomaliesProcessed.Count);
            SUIManager.UpdateTimerUI(timer);

            // Kiểm tra xem game có kết thúc không sau mỗi lần tìm thấy anomaly
            CheckGameOver();
        }
        else
        {
            Debug.LogWarning("Anomaly is null or already processed.");
        }
    }

    // Phương thức phục hồi pin
    private void RestoreBatteryLevel(int amount)
    {
        // Nếu mức pin chưa đạt max (3), cộng thêm amount vào mức pin
        if (scameraUI.batteryLevel < 3)
        {
            scameraUI.batteryLevel += amount;

            // Đảm bảo mức pin không vượt quá giới hạn (max là 3)
            scameraUI.batteryLevel = Mathf.Min(scameraUI.batteryLevel, 3);

            // Cập nhật UI mức pin
            scameraUI.UpdateBatteryUI();
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
        SUIManager.ShowEndGameUI(victory); // Gọi phương thức để hiển thị kết quả thắng/thua
    }

    // Phương thức reset lại game
    public void ResetGame()
    {
        gameEnded = false;
        anomaliesFound = 0;
        timer = gameDuration;
        anomaliesProcessed.Clear();

        SUIManager.UpdateAnomalyCountUI(anomaliesFound, totalAnomalies);
        SUIManager.UpdateTimerUI(timer);

        Debug.Log("Game has been reset.");
    }

    // Phương thức quay lại menu chính khi bấm nút
    public void OnClickBackToMenu()
    {
        Debug.Log("Back to the main menu.");
        ResetGame();
        Loader.Load(Loader.Scene.LoadingScene, Loader.Scene.MainMenuScene); // Load lại cảnh menu chính
    }

    // Phương thức chơi lại game
    public void OnClickReplay()
    {
        Debug.Log("Replay the game.");
        ResetGame();
        Loader.Load(Loader.Scene.LoadingScene, Loader.Scene.SingleplayerScene); // Load lại cảnh game hiện tại
    }

    private void UnlockCursor()
    {
        // Mở khóa con trỏ khi cần thiết
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
