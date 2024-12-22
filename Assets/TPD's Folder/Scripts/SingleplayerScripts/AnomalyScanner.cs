using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnomalyScanner : MonoBehaviour
{
    public Transform player; // Tham chiếu đến player
    public float scanRadius = 20f; // Bán kính quét xung quanh player
    public SUserInterfaceManager SUIManager; // Tham chiếu đến UIManager để cập nhật UI

    void Update()
    {
        // Quét các Anomaly trong bán kính scanRadius xung quanh player
        ScanForAnomalies();
    }

    void ScanForAnomalies()
    {
        // Lấy tất cả các collider trong bán kính scanRadius xung quanh player
        Collider[] anomaliesInRange = Physics.OverlapSphere(player.position, scanRadius);

        bool foundAnomaly = false; // Biến để kiểm tra xem có anomaly nào không

        // Kiểm tra từng anomaly tìm được và thực hiện hành động cần thiết
        foreach (Collider anomaly in anomaliesInRange)
        {
            // Kiểm tra nếu collider có tag "Anomaly"
            if (anomaly.CompareTag("Anomaly"))
            {
                foundAnomaly = true; // Đã tìm thấy anomaly
                Debug.Log("Found Anomaly: " + anomaly.gameObject.name);
                break; // Nếu tìm thấy anomaly, thoát khỏi vòng lặp
            }
        }

        // Cập nhật UI tùy vào việc tìm thấy anomaly hay không
        if (foundAnomaly)
        {
            SUIManager.AnomalyScannedText.text = "Có anomaly trong khu vực";
        }
        else
        {
            SUIManager.AnomalyScannedText.text = "Không có anomaly trong khu vực";
        }
    }

    // Hiển thị bán kính quét trong editor (tùy chọn)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, scanRadius);
    }
}
