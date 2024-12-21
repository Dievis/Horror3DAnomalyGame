using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AnomalySpawnManager : MonoBehaviourPunCallbacks
{
    public GameObject[] anomalyPrefabs;  // Prefab của anomaly cần spawn
    public Transform[] spawnPoints;      // Các điểm spawn trên bản đồ
    private List<GameObject> spawnedAnomalies = new List<GameObject>();

    void Start()
    {
        // Kiểm tra nếu là MasterClient, sẽ spawn anomalies và sync cho tất cả các client
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[AnomalySpawnManager] MasterClient is spawning anomalies...");
            SpawnAnomalies();
        }
        else
        {
            Debug.Log("[AnomalySpawnManager] This is a regular client, waiting for anomalies to spawn.");
        }
    }

    // Phương thức này sẽ spawn anomalies và đồng bộ hóa trên tất cả các client
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

        foreach (GameObject anomalyPrefab in anomalyPrefabs)
        {
            Transform spawnPoint = GetRandomSpawnPoint();

            if (spawnPoint != null)
            {
                Debug.Log($"Spawning anomaly: {anomalyPrefab.name} at {spawnPoint.position}");

                // Spawn anomaly sử dụng PhotonNetwork.Instantiate
                GameObject anomaly = PhotonNetwork.Instantiate(anomalyPrefab.name, spawnPoint.position, spawnPoint.rotation, 0);
                spawnedAnomalies.Add(anomaly);
            }
            else
            {
                Debug.LogWarning("No available spawn points.");
                break;
            }
        }
    }

    // Phương thức này sẽ được gọi khi cần đồng bộ hóa các anomalies cho các client mới kết nối
    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RequestAnomalySync", PhotonNetwork.MasterClient);
        }
    }

    // RPC để yêu cầu đồng bộ hóa các anomalies hiện có
    [PunRPC]
    private void RequestAnomalySync()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (GameObject anomaly in spawnedAnomalies)
            {
                // Đồng bộ thông tin về anomaly, không cần instantiating lại
                photonView.RPC("SyncAnomaly", RpcTarget.Others, anomaly.name, anomaly.transform.position, anomaly.transform.rotation);
            }
        }
    }

    // RPC để đồng bộ hóa anomalies trên các client khác
    [PunRPC]
    private void SyncAnomaly(string anomalyName, Vector3 position, Quaternion rotation)
    {
        // Chỉ cần spawn anomaly một lần trên client, không cần tạo lại.
        if (!GameObject.Find(anomalyName))  // Kiểm tra xem anomaly đã tồn tại trên client chưa nếu chưa thì thực hiện lệnh dưới
        {
            PhotonNetwork.Instantiate(anomalyName, position, rotation, 0);
        }
    }

    private Transform GetRandomSpawnPoint()
    {
        if (spawnPoints.Length == 0) return null;
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
}
