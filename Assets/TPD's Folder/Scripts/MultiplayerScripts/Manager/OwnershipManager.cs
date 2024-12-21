using Photon.Pun;
using UnityEngine;

public class OwnershipManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        // Chỉ Master Client mới thực hiện việc chuyển quyền sở hữu
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[OwnershipManager] This is the MasterClient. Starting ownership transfer...");
            TransferOwnershipToClients();
        }
        else
        {
            Debug.Log("[OwnershipManager] This is not the MasterClient. No action will be taken.");
        }
    }

    private void TransferOwnershipToClients()
    {
        // Nếu chỉ có Master Client trong phòng, không cần chuyển quyền sở hữu
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
        {
            Debug.Log("[OwnershipManager] Only MasterClient is in the room. Skipping ownership transfer.");
            return;
        }

        // Lấy tất cả các object có tag "Anomaly"
        GameObject[] allObjectsInScene = GameObject.FindGameObjectsWithTag("Anomaly");
        Debug.Log($"[OwnershipManager] Found {allObjectsInScene.Length} objects with tag 'Anomaly'.");

        int successCount = 0; // Đếm số lượng chuyển quyền thành công

        // Duyệt qua tất cả các object trong scene
        foreach (GameObject obj in allObjectsInScene)
        {
            PhotonView photonView = obj.GetComponent<PhotonView>();

            // Kiểm tra nếu object có PhotonView
            if (photonView != null)
            {
                Debug.Log($"[OwnershipManager] Attempting to transfer ownership of object: {obj.name}");

                // Chỉ chuyển quyền sở hữu nếu object hiện thuộc về Master Client
                if (photonView.Owner == PhotonNetwork.MasterClient)
                {
                    // Lấy danh sách client không phải Master Client
                    var clients = PhotonNetwork.PlayerListOthers;

                    // Chuyển quyền sở hữu cho client đầu tiên trong danh sách
                    if (clients.Length > 0)
                    {
                        photonView.TransferOwnership(clients[0]);
                        Debug.Log($"[OwnershipManager] Ownership of {obj.name} successfully transferred to {clients[0].NickName}.");
                        successCount++;
                    }
                    else
                    {
                        Debug.LogWarning($"[OwnershipManager] No valid client found to transfer ownership of {obj.name}.");
                    }
                }
                else
                {
                    Debug.Log($"[OwnershipManager] Skipped transferring ownership of {obj.name} as it is not owned by Master Client.");
                }
            }
            else
            {
                Debug.LogError($"[OwnershipManager] Object {obj.name} does not have a PhotonView component.");
            }
        }

        Debug.Log($"[OwnershipManager] Successfully transferred ownership of {successCount}/{allObjectsInScene.Length} objects.");
    }


    // Đảm bảo gọi lại khi có người chơi mới vào phòng
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log($"[OwnershipManager] A new player entered the room: {newPlayer.NickName}");

        // Master Client sẽ gọi lại khi có người chơi mới vào phòng
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[OwnershipManager] MasterClient is re-assigning ownership to all clients...");
            TransferOwnershipToClients();
        }
    }
}
