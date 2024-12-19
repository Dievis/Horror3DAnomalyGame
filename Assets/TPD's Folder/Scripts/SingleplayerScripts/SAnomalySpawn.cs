using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAnomalySpawn : MonoBehaviour
{
    public GameObject[] anomalyPrefabs;  // Prefab của anomaly cần spawn
    public Transform[] spawnPoints;      // Các điểm spawn trên bản đồ
    private List<GameObject> spawnedAnomalies = new List<GameObject>();
    private List<int> usedSpawnIndices = new List<int>();  // Danh sách các vị trí spawn đã được sử dụng

    void Start()
    {
        // Kiểm tra và spawn anomalies khi game bắt đầu
        SpawnAnomalies();
    }

    // Phương thức này sẽ spawn anomalies và đảm bảo không spawn trùng vị trí
    private void SpawnAnomalies()
    {
        Debug.Log("Spawning anomalies...");

        // Kiểm tra trước khi spawn anomalies
        if (anomalyPrefabs == null || anomalyPrefabs.Length == 0)
        {
            Debug.LogError("No anomaly prefabs assigned!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        // Xóa danh sách vị trí đã sử dụng để có thể spawn lại trong các lần chơi sau
        usedSpawnIndices.Clear();

        foreach (GameObject anomalyPrefab in anomalyPrefabs)
        {
            Transform spawnPoint = GetRandomSpawnPoint();

            if (spawnPoint != null)
            {
                Debug.Log($"Spawning anomaly: {anomalyPrefab.name} at {spawnPoint.position}");

                // Spawn anomaly
                GameObject anomaly = Instantiate(anomalyPrefab, spawnPoint.position, spawnPoint.rotation);
                spawnedAnomalies.Add(anomaly);
            }
            else
            {
                Debug.LogWarning("No available spawn points.");
                break;
            }
        }
    }

    // Lấy vị trí spawn ngẫu nhiên, đảm bảo không trùng lặp
    private Transform GetRandomSpawnPoint()
    {
        if (spawnPoints.Length == 0) return null;

        // Tạo một danh sách chứa các chỉ số vị trí spawn chưa được sử dụng
        List<int> availableIndices = new List<int>();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (!usedSpawnIndices.Contains(i))
            {
                availableIndices.Add(i);
            }
        }

        if (availableIndices.Count == 0) return null;  // Không còn vị trí spawn chưa sử dụng

        // Chọn một vị trí ngẫu nhiên từ danh sách các vị trí chưa được sử dụng
        int randomIndex = availableIndices[Random.Range(0, availableIndices.Count)];
        usedSpawnIndices.Add(randomIndex);  // Đánh dấu vị trí đã sử dụng

        return spawnPoints[randomIndex];
    }
}
