using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAnomalySpawn : MonoBehaviour
{
    public GameObject[] anomalyPrefabs;  // Prefab của anomaly cần spawn
    public Transform[] spawnPoints;      // Các điểm spawn trên bản đồ
    private List<GameObject> spawnedAnomalies = new List<GameObject>();
    private HashSet<int> usedSpawnIndices = new HashSet<int>();  // Sử dụng HashSet để đảm bảo không trùng lặp

    void Start()
    {
        // Kiểm tra và spawn anomalies khi game bắt đầu
        SpawnAnomalies();
    }

    private void SpawnAnomalies()
    {
        Debug.Log("Spawning anomalies...");

        // Kiểm tra điều kiện trước khi spawn anomalies
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

        // Xóa danh sách các vị trí đã sử dụng và anomalies đã spawn (nếu cần spawn lại)
        usedSpawnIndices.Clear();
        foreach (GameObject anomaly in spawnedAnomalies)
        {
            if (anomaly != null) Destroy(anomaly);
        }
        spawnedAnomalies.Clear();

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

    private Transform GetRandomSpawnPoint()
    {
        if (spawnPoints.Length == 0) return null;

        // Tạo danh sách chỉ số vị trí chưa sử dụng
        List<int> availableIndices = new List<int>();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (!usedSpawnIndices.Contains(i))
            {
                availableIndices.Add(i);
            }
        }

        if (availableIndices.Count == 0)
        {
            Debug.LogWarning("All spawn points are used!");
            return null;  // Không còn vị trí spawn chưa sử dụng
        }

        // Chọn một vị trí ngẫu nhiên từ danh sách các vị trí chưa sử dụng
        int randomIndex = availableIndices[Random.Range(0, availableIndices.Count)];
        usedSpawnIndices.Add(randomIndex);  // Đánh dấu vị trí đã sử dụng

        return spawnPoints[randomIndex];
    }
}
