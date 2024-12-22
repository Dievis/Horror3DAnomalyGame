using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AnomalySpawnManager : MonoBehaviourPunCallbacks
{
    public GameObject[] anomalyPrefabs;  // Mảng chứa các prefab của anomaly cần spawn
    public Transform[] spawnPoints;      // Mảng chứa các điểm spawn trên bản đồ
    private List<GameObject> spawnedAnomalies = new List<GameObject>();  // Danh sách chứa các anomalies đã spawn
    private HashSet<Transform> occupiedSpawnPoints = new HashSet<Transform>();  // Tập hợp chứa các điểm spawn đã được sử dụng

    void Start()
    {
        // Kiểm tra nếu là MasterClient, nếu đúng thì sẽ spawn anomalies và đồng bộ cho tất cả các client
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[AnomalySpawnManager] MasterClient is spawning anomalies...");
            SpawnAnomalies();  // Gọi hàm spawn anomalies
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

        // Kiểm tra xem mảng anomalyPrefabs có được gán giá trị hay không
        if (anomalyPrefabs == null || anomalyPrefabs.Length == 0)
        {
            Debug.LogError("No anomaly prefabs assigned!");  // Thông báo lỗi nếu không có prefab
            return;
        }

        // Kiểm tra xem mảng spawnPoints có được gán giá trị hay không
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");  // Thông báo lỗi nếu không có điểm spawn
            return;
        }

        // Duyệt qua từng prefab anomaly để spawn
        foreach (GameObject anomalyPrefab in anomalyPrefabs)
        {
            // Lấy một điểm spawn ngẫu nhiên chưa bị chiếm
            Transform spawnPoint = GetRandomAvailableSpawnPoint();

            if (spawnPoint != null)
            {
                Debug.Log($"Spawning anomaly: {anomalyPrefab.name} at {spawnPoint.position}");

                // Spawn anomaly sử dụng PhotonNetwork.Instantiate để đồng bộ trên tất cả client
                GameObject anomaly = PhotonNetwork.Instantiate(anomalyPrefab.name, spawnPoint.position, spawnPoint.rotation, 0);
                spawnedAnomalies.Add(anomaly);  // Thêm anomaly vào danh sách đã spawn

                // Đánh dấu điểm spawn này là đã được sử dụng
                occupiedSpawnPoints.Add(spawnPoint);
            }
            else
            {
                Debug.LogWarning("No available spawn points.");  // Cảnh báo nếu không còn điểm spawn khả dụng
                break;
            }
        }
    }

    // Phương thức này sẽ được gọi khi một client mới tham gia phòng, yêu cầu đồng bộ anomalies
    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)  // Nếu không phải MasterClient thì yêu cầu đồng bộ
        {
            photonView.RPC("RequestAnomalySync", PhotonNetwork.MasterClient);
        }
    }

    // RPC để yêu cầu đồng bộ hóa các anomalies hiện có từ MasterClient
    [PunRPC]
    private void RequestAnomalySync()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (GameObject anomaly in spawnedAnomalies)
            {
                // Đồng bộ thông tin về anomaly, không cần phải instantiate lại
                photonView.RPC("SyncAnomaly", RpcTarget.Others, anomaly.name, anomaly.transform.position, anomaly.transform.rotation);
            }
        }
    }

    // RPC để đồng bộ hóa anomalies trên các client khác
    [PunRPC]
    private void SyncAnomaly(string anomalyName, Vector3 position, Quaternion rotation)
    {
        // Chỉ spawn anomaly nếu nó chưa tồn tại trên client
        if (!GameObject.Find(anomalyName))  // Kiểm tra xem anomaly đã tồn tại trên client chưa
        {
            PhotonNetwork.Instantiate(anomalyName, position, rotation, 0);  // Spawn anomaly trên client
        }
    }

    // Phương thức này trả về một điểm spawn ngẫu nhiên chưa bị chiếm
    private Transform GetRandomAvailableSpawnPoint()
    {
        // Tạo một danh sách các điểm spawn khả dụng
        List<Transform> availableSpawnPoints = new List<Transform>();
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (!occupiedSpawnPoints.Contains(spawnPoint))  // Kiểm tra nếu điểm spawn chưa bị chiếm
            {
                availableSpawnPoints.Add(spawnPoint);  // Thêm điểm spawn vào danh sách khả dụng
            }
        }

        if (availableSpawnPoints.Count == 0)
        {
            return null;  // Nếu không còn điểm spawn khả dụng thì trả về null
        }

        // Chọn ngẫu nhiên một điểm spawn trong danh sách
        return availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];
    }
}
