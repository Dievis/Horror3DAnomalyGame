using UnityEngine;
using Photon.Pun;

public class PlayerInventory : MonoBehaviourPunCallbacks
{
    private string[] inventory = new string[2]; // Inventory có 2 slot

    public Transform handTransform; // Vị trí để "cầm" đồ khi nhặt
    public LayerMask interactableLayerMask; // Thêm trường LayerMask để chọn layer từ ngoài

    private void Start()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i] = null; // Ban đầu các slot đều trống
        }
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        // Kiểm tra nếu nhấn phím nhặt (ví dụ phím E)
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickupItem();
        }
    }

    private void TryPickupItem()
    {
        // Tạo raycast từ vị trí camera, hướng theo hướng camera đang nhìn
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        // Dùng LayerMask để loại trừ các đối tượng không liên quan (dựa vào LayerMask từ Inspector)
        if (Physics.Raycast(ray, out RaycastHit hit, 5f, interactableLayerMask)) // Dùng interactableLayerMask ở đây
        {
            Debug.Log("Đã raycast và chạm vào: " + hit.collider.name);

            if (hit.collider.CompareTag("PickupItem"))
            {
                Item item = hit.collider.GetComponent<Item>();
                if (item != null && item.isAvailable)
                {
                    photonView.RPC("PickupItem", RpcTarget.MasterClient, item.gameObject.GetComponent<PhotonView>().ViewID);
                }
            }
        }
    }

    [PunRPC]
    private void PickupItem(int itemViewID)
    {
        GameObject itemObject = PhotonView.Find(itemViewID).gameObject;
        Item item = itemObject.GetComponent<Item>();
        if (item == null || !item.isAvailable) return;

        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
            {
                // Thêm item vào inventory
                inventory[i] = item.itemName;

                // Cập nhật trạng thái item
                item.isAvailable = false;
                photonView.RPC("AttachItemToHand", RpcTarget.All, itemViewID);
                return;
            }
        }

        Debug.Log("Inventory đầy, không thể nhặt thêm.");
    }

    [PunRPC]
    private void AttachItemToHand(int itemViewID)
    {
        GameObject itemObject = PhotonView.Find(itemViewID).gameObject;
        Rigidbody rb = itemObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true; // Đảm bảo item không bị ảnh hưởng bởi vật lý khi cầm
        }

        // Tắt collider khi cầm
        Collider collider = itemObject.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Đặt item vào tay người chơi
        itemObject.transform.SetParent(handTransform);
        itemObject.transform.localPosition = Vector3.zero;
        itemObject.transform.localRotation = Quaternion.identity;
    }

    public void DropItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventory.Length || inventory[slotIndex] == null)
        {
            Debug.Log("Không có item trong slot này để thả.");
            return;
        }

        photonView.RPC("DropItem", RpcTarget.MasterClient, slotIndex);
    }

    [PunRPC]
    private void DropItemRPC(int slotIndex)
    {
        string itemName = inventory[slotIndex];
        inventory[slotIndex] = null;

        GameObject droppedItem = ItemManager.Instance.SpawnItem(itemName, handTransform.position);
        photonView.RPC("SpawnDroppedItem", RpcTarget.All, droppedItem.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    private void SpawnDroppedItem(int itemViewID)
    {
        GameObject itemObject = PhotonView.Find(itemViewID).gameObject;
        Rigidbody rb = itemObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false; // Bật vật lý khi thả item
        }

        // Bật lại collider khi item bị thả
        Collider collider = itemObject.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        // Đặt lại item vào thế giới
        itemObject.transform.SetParent(null);
    }
}
