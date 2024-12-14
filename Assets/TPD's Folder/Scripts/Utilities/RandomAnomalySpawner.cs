using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RandomAnomalySpawner : MonoBehaviourPunCallbacks
{
    public GameObject[] anomalyPrefabs; // Các prefab anomaly
    public Transform[] spawnPoints;    // Các vị trí spawn anomaly

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[RandomAnomalySpawner] MasterClient is spawning anomalies...");
            SpawnAnomalies();
        }
    }

    private void SpawnAnomalies()
    {
        if (anomalyPrefabs == null || anomalyPrefabs.Length == 0)
        {
            Debug.LogError("[RandomAnomalySpawner] No anomaly prefabs assigned!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[RandomAnomalySpawner] No spawn points assigned!");
            return;
        }

        // Lưu trữ các vị trí đã spawn để tránh trùng lặp
        List<Transform> usedSpawnPoints = new List<Transform>();

        foreach (GameObject anomalyPrefab in anomalyPrefabs)
        {
            Transform spawnPoint = GetRandomSpawnPoint(usedSpawnPoints);

            if (spawnPoint != null)
            {
                // Tạo anomaly tại vị trí ngẫu nhiên và đồng bộ qua Photon
                GameObject spawnedAnomaly = PhotonNetwork.Instantiate(anomalyPrefab.name, spawnPoint.position, spawnPoint.rotation);
                Debug.Log($"[RandomAnomalySpawner] Spawned {anomalyPrefab.name} at {spawnPoint.position}");
            }
            else
            {
                Debug.LogWarning("[RandomAnomalySpawner] No more available spawn points.");
                break;
            }
        }
    }

    private Transform GetRandomSpawnPoint(List<Transform> usedSpawnPoints)
    {
        List<Transform> availablePoints = new List<Transform>(spawnPoints);

        // Loại bỏ các điểm đã sử dụng
        foreach (Transform usedPoint in usedSpawnPoints)
        {
            availablePoints.Remove(usedPoint);
        }

        if (availablePoints.Count > 0)
        {
            Transform selectedPoint = availablePoints[Random.Range(0, availablePoints.Count)];
            usedSpawnPoints.Add(selectedPoint);
            return selectedPoint;
        }

        return null;
    }
}
