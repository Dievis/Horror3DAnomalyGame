using Photon.Pun;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    public GameObject[] itemPrefabs; // Danh sách prefab các item

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public GameObject SpawnItem(string itemName, Vector3 position)
    {
        foreach (var prefab in itemPrefabs)
        {
            if (prefab.GetComponent<Item>().itemName == itemName)
            {
                GameObject spawnedItem = PhotonNetwork.Instantiate(prefab.name, position, Quaternion.identity);

                // Cài đặt Rigidbody của item
                Rigidbody rb = spawnedItem.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Điều chỉnh thông số Rigidbody
                    rb.useGravity = true; // Bật trọng lực
                    rb.isKinematic = false; // Tắt chế độ kinematic khi item rơi ra ngoài
                    rb.mass = 1f; // Khối lượng item
                    rb.drag = 0.5f; // Ma sát tuyến tính
                    rb.angularDrag = 0.05f; // Ma sát góc
                }

                return spawnedItem;
            }
        }

        Debug.LogError("Không tìm thấy prefab cho item: " + itemName);
        return null;
    }
}
