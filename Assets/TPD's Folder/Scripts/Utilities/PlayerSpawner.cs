using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab; // Prefab của nhân vật
    public Transform[] spawnPoints; // Các vị trí spawn nhân vật

    private List<int> usedSpawnIndices = new List<int>(); // Danh sách các vị trí spawn đã được sử dụng

    private void Start()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player Prefab chưa được gắn trong GameManager!");
            return;
        }

        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
        else
        {
            Debug.LogWarning("PhotonNetwork không kết nối!");
        }
    }

    void SpawnPlayer()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("Không có vị trí spawn nào được gắn trong PlayerSpawner!");
            return;
        }

        if (usedSpawnIndices.Count >= spawnPoints.Length)
        {
            Debug.LogWarning("Tất cả vị trí spawn đã được sử dụng! Sẽ spawn ngẫu nhiên tại vị trí bất kỳ.");
            usedSpawnIndices.Clear(); // Xóa danh sách nếu tất cả vị trí đã được sử dụng
        }

        int spawnIndex;
        do
        {
            spawnIndex = Random.Range(0, spawnPoints.Length);
        } while (usedSpawnIndices.Contains(spawnIndex));

        usedSpawnIndices.Add(spawnIndex); // Thêm vị trí đã sử dụng vào danh sách

        Transform spawnPoint = spawnPoints[spawnIndex];

        // Tạo nhân vật và đồng bộ qua mạng
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }
}
