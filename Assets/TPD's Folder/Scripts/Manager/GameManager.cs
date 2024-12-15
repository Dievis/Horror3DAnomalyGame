using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Game Stats")]
    [SerializeField] private int totalAnomalies;        // Tổng số anomalies
    [SerializeField] private int anomaliesFound = 0;     // Số anomalies đã tìm thấy
    [SerializeField] private float gameDuration = 60f;   // Thời gian game
    [SerializeField] private float timer;               // Bộ đếm thời gian
    private bool gameEnded = false;                      // Cờ kết thúc game

    private UIManager uiManager;

    private void Start()
    {
        PhotonView photonView = GetComponent<PhotonView>();
        // Khởi tạo UIManager và các giá trị ban đầu
        uiManager = FindObjectOfType<UIManager>();
        timer = gameDuration;

        // Cập nhật tổng số anomalies từ số lượng GameObject có tag "Anomaly"
        totalAnomalies = GameObject.FindGameObjectsWithTag("Anomaly").Length;

        // Cập nhật UI ban đầu
        uiManager.UpdateAnomalyCountUI(anomaliesFound, totalAnomalies);
        uiManager.UpdateTimerUI(timer);
    }

    private void Update()
    {
        if (gameEnded) return;

        // Giảm thời gian mỗi frame
        timer -= Time.deltaTime;
        uiManager.UpdateTimerUI(timer);

        // Kết thúc game nếu thời gian còn lại <= 0
        if (timer <= 0f) EndGame(false);

        // Nếu không còn anomaly nào, game thắng
        if (totalAnomalies <= 0) EndGame(true);

        // Kiểm tra nhấn phím Tab để hiển thị/ẩn thông tin game
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            uiManager.ToggleMatchInfoPanel();
        }
    }

    // RPC để thông báo khi một anomaly bị tìm thấy và xóa
    [PunRPC]
    public void NotifyAnomalyFound()
    {
        anomaliesFound++;  // Tăng số lượng anomaly đã tìm thấy
        totalAnomalies--;  // Giảm tổng số anomaly còn lại

        Debug.Log("Anomaly found! Total anomalies found: " + anomaliesFound);

        // Cập nhật UI với số anomalies đã tìm thấy và còn lại
        uiManager.UpdateAnomalyCountUI(anomaliesFound, totalAnomalies);

        // Nếu không còn anomaly nào, game thắng
        if (totalAnomalies <= 0)
        {
            EndGame(true);
        }
    }

    // Phương thức này được gọi khi một anomaly bị xóa
    public void OnAnomalyDestroyed()
    {
        totalAnomalies--;  // Giảm tổng số anomalies khi một anomaly biến mất
        Debug.Log("Anomaly destroyed! Remaining anomalies: " + totalAnomalies);

        // Cập nhật UI với số anomalies còn lại
        uiManager.UpdateAnomalyCountUI(anomaliesFound, totalAnomalies);

        // Nếu không còn anomaly nào, game thắng
        if (totalAnomalies <= 0)
        {
            EndGame(true);
        }
    }

    private void EndGame(bool victory)
    {
        gameEnded = true;
        uiManager.ShowEndGameUI(victory);  // Hiển thị UI kết thúc game
    }
}
