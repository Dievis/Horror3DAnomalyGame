using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab; // Prefab của nhân vật
    public Transform[] spawnPoints; // Các vị trí spawn nhân vật

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
        // Lựa chọn ngẫu nhiên vị trí spawn
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Tạo nhân vật và đồng bộ qua mạng
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }

}
