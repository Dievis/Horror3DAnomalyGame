using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Game Stats")]
    [SerializeField] private int totalAnomalies;        // Tổng số anomalies
    [SerializeField] private int anomaliesFound = 0;     // Số anomalies đã tìm thấy
    [SerializeField] private float gameDuration = 60f;   // Thời gian game
    [SerializeField] private float timer;               // Bộ đếm thời gian
    [SerializeField][Range(0f, 1f)] private float victoryThreshold = 0.8f; // Tỷ lệ anomaly cần tìm để thắng
    private bool gameEnded = false;

    // Singleton instance
    public static GameManager instance;

    private UIManager uiManager;
    private HashSet<GameObject> anomaliesProcessed = new HashSet<GameObject>();  // Set các anomalies đã được xử lý (tìm thấy hoặc tiêu diệt)

    private void Awake()
    {
        // Khởi tạo Singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        timer = gameDuration;

        // Reset UI ngay lập tức để đảm bảo không có UI bị hiển thị sai
        uiManager.ResetUI();

        // Cập nhật lại UI ban đầu
        uiManager.UpdateAnomalyCountUI(anomaliesFound, totalAnomalies);
        uiManager.UpdateTimerUI(timer);

        // Bắt đầu game với anomalies
        StartCoroutine(WaitForAnomaliesToSpawn());
    }

    // Coroutine để chờ anomalies spawn
    private IEnumerator WaitForAnomaliesToSpawn()
    {
        // Chờ một khoảng thời gian nhất định để anomalies spawn
        float waitTime = 5f; // Ví dụ chờ 5 giây
        yield return new WaitForSeconds(waitTime);

        // Kiểm tra tổng số anomalies sau khi chờ
        totalAnomalies = GameObject.FindGameObjectsWithTag("Anomaly").Length;

        if (totalAnomalies > 0)
        {
            Debug.Log("Game started! Total anomalies: " + totalAnomalies);
            uiManager.UpdateAnomalyCountUI(anomaliesFound, totalAnomalies);
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

        timer -= Time.deltaTime;
        uiManager.UpdateTimerUI(timer);

        if (timer <= 0f) EndGame(false);

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            uiManager.ToggleMatchInfoPanel();
        }
    }

    // Phương thức thông báo tìm thấy anomaly
    [PunRPC]
    public void NotifyAnomalyFound(int anomalyViewID)
    {
        GameObject anomaly = PhotonView.Find(anomalyViewID)?.gameObject;
        if (anomaly != null && !anomaliesProcessed.Contains(anomaly))
        {
            anomaliesProcessed.Add(anomaly);
            anomaliesFound++;
            Debug.Log("Anomaly found! Total anomalies found: " + anomaliesFound);

            uiManager.UpdateAnomalyCountUI(anomaliesFound, totalAnomalies - anomaliesProcessed.Count);

            photonView.RPC("CheckGameOver", RpcTarget.All);
        }
        else
        {
            Debug.LogWarning("Anomaly is null or already processed.");
        }
    }

    [PunRPC]
    private void CheckGameOver()
    {
        float requiredAnomalies = totalAnomalies * victoryThreshold;
        if (anomaliesFound >= requiredAnomalies)
        {
            EndGame(true);  // Chiến thắng khi đạt tỷ lệ anomalies yêu cầu
        }
        else if (timer <= 0f)
        {
            EndGame(false);  // Thua khi hết thời gian
        }
    }

    private void EndGame(bool victory)
    {
        if (gameEnded) return;

        gameEnded = true;
        photonView.RPC("ShowEndGameUI", RpcTarget.All, victory);
    }

    public void ShowEndGameUI(bool victory)
    {
        uiManager.ShowEndGameUI(victory); // Hiển thị UI chiến thắng/thua
    }

    public void ResetGame()
    {
        gameEnded = false;
        anomaliesFound = 0;
        timer = gameDuration;
        anomaliesProcessed.Clear();

        uiManager.UpdateAnomalyCountUI(anomaliesFound, totalAnomalies);
        uiManager.UpdateTimerUI(timer);
        Debug.Log("Game has been reset.");
    }

    public void OnClickBackToMenu()
    {
        Debug.Log("Quay lại menu chính.");
        PhotonNetwork.LeaveRoom(); // Thoát khỏi phòng
        ResetGame();
        Loader.Load(Loader.Scene.LoadingScene, Loader.Scene.MainMenuScene); // Load scene menu chính
    }
}
