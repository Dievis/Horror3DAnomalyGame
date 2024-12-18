using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGameManager : MonoBehaviour
{
    [Header("Game Stats")]
    [SerializeField] private int totalAnomalies;        // Tổng số anomalies trong game
    [SerializeField] private int anomaliesFound = 0;     // Số anomalies đã tìm thấy
    [SerializeField][Range(0f, 1f)] private float victoryThreshold = 0.8f; // Tỷ lệ chiến thắng (phần trăm anomalies cần tìm để thắng)
    private bool gameEnded = false;                      // Trạng thái game (đã kết thúc hay chưa)

    public float gameDuration = 60f;   // Thời gian của game
    public float timer;               // Bộ đếm thời gian

    // Singleton instance (dùng để quản lý đối tượng game manager)
    public static SGameManager instance;

    private SUserInterfaceManager SUIManager;  // Quản lý giao diện người dùng
    private HashSet<GameObject> anomaliesProcessed = new HashSet<GameObject>();  // Tập hợp các anomalies đã được xử lý (tìm thấy hoặc đã xóa)

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
        SUIManager = FindObjectOfType<SUserInterfaceManager>();  // Tìm và gán SUserInterfaceManager
        timer = gameDuration;  // Khởi tạo thời gian

        // Hiển thị Panel Hướng dẫn khi game bắt đầu
        SUIManager.ShowTutorialPanel();

        // Reset UI ngay lập tức để tránh hiển thị sai thông tin
        SUIManager.ResetUI();

        // Cập nhật UI ban đầu
        SUIManager.UpdateAnomalyCountUI(anomaliesFound, totalAnomalies);
        SUIManager.UpdateTimerUI(timer);

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
            Debug.Log("Game started! Total anomalies: " + totalAnomalies);
            SUIManager.UpdateAnomalyCountUI(anomaliesFound, totalAnomalies);

            // Gọi HideTutorialPanel khi tất cả anomalies đã spawn xong
            SUIManager.HideTutorialPanel();
        }
        else
        {
            Debug.LogWarning("No anomalies found after waiting. Game will not start.");
            // Nếu không có anomalies, kết thúc game ngay lập tức
            EndGame(false);
        }
    }

    private void Update()
    {
        if (gameEnded) return;

        // Giảm thời gian
        timer -= Time.deltaTime;
        SUIManager.UpdateTimerUI(timer);

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

            // Cập nhật UI số lượng anomalies đã tìm thấy
            SUIManager.UpdateAnomalyCountUI(anomaliesFound, totalAnomalies - anomaliesProcessed.Count);

            // Kiểm tra xem game có kết thúc không sau mỗi lần tìm thấy anomaly
            CheckGameOver();
        }
        else
        {
            Debug.LogWarning("Anomaly is null or already processed.");
        }
    }

    // Kiểm tra điều kiện kết thúc game
    private void CheckGameOver()
    {
        float requiredAnomalies = totalAnomalies * victoryThreshold;  // Tính toán số anomalies cần thiết để thắng
        if (anomaliesFound >= requiredAnomalies)
        {
            UnlockCursor(); // Mở khóa trỏ chuột để tương tác khi chiến thắng
            EndGame(true);  // Chiến thắng nếu số anomalies cần tìm đã đủ
        }
        else if (timer <= 0f)
        {
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
        SUIManager.ShowEndGameUI(victory);  // Gọi phương thức để hiển thị kết quả thắng/thua
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
    private void UnlockCursor()
    {
        // Mở khóa con trỏ khi cần thiết
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
