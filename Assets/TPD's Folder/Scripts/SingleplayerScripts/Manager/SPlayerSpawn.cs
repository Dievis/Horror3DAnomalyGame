using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPlayerSpawn : MonoBehaviour
{
    public GameObject playerPrefab; // Prefab của nhân vật
    public Transform[] spawnPoints; // Các vị trí spawn nhân vật

    private List<int> usedSpawnIndices = new List<int>(); // Danh sách các vị trí spawn đã được sử dụng

    // Hàm Start được gọi khi script bắt đầu chạy
    private void Start()
    {
        if (playerPrefab == null) // Kiểm tra nếu prefab nhân vật chưa được gắn
        {
            Debug.LogError("Player Prefab chưa được gắn trong GameManager!"); // In lỗi nếu prefab chưa được gắn
            return;
        }

        SpawnPlayer(); // Gọi hàm SpawnPlayer để tạo nhân vật
    }

    // Hàm SpawnPlayer để tạo nhân vật tại vị trí spawn ngẫu nhiên
    void SpawnPlayer()
    {
        if (spawnPoints.Length == 0) // Kiểm tra nếu không có vị trí spawn nào được gắn
        {
            Debug.LogError("Không có vị trí spawn nào được gắn trong PlayerSpawner!"); // In lỗi nếu không có vị trí spawn
            return;
        }

        if (usedSpawnIndices.Count >= spawnPoints.Length) // Kiểm tra nếu tất cả các vị trí spawn đã được sử dụng
        {
            Debug.LogWarning("Tất cả vị trí spawn đã được sử dụng! Sẽ spawn ngẫu nhiên tại vị trí bất kỳ."); // Cảnh báo và thông báo sẽ chọn ngẫu nhiên vị trí spawn
            usedSpawnIndices.Clear(); // Xóa danh sách các vị trí đã sử dụng để có thể spawn lại từ đầu
        }

        int spawnIndex;
        do
        {
            spawnIndex = Random.Range(0, spawnPoints.Length); // Chọn ngẫu nhiên một vị trí spawn
        } while (usedSpawnIndices.Contains(spawnIndex)); // Đảm bảo rằng vị trí spawn chưa được sử dụng

        usedSpawnIndices.Add(spawnIndex); // Thêm vị trí spawn đã chọn vào danh sách

        Transform spawnPoint = spawnPoints[spawnIndex]; // Lấy vị trí spawn tương ứng

        // Tạo nhân vật tại vị trí spawn đã chọn
        Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
