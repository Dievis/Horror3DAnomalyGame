using Photon.Pun;
using UnityEngine;

public class OwnershipManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        // Chỉ Master Client mới thực hiện việc chuyển quyền sở hữu
        if (PhotonNetwork.IsMasterClient)
        {
            // Khi scene bắt đầu, Master Client sẽ chuyển quyền sở hữu cho tất cả các object trong scene
            TransferOwnershipToAllClients();
        }
    }

    // RPC để chuyển quyền sở hữu của object cho tất cả các client
    [PunRPC]
    public void TransferOwnershipToAllClients()
    {
        // Lấy tất cả các object trong scene
        GameObject[] allObjectsInScene = GameObject.FindGameObjectsWithTag("Anomaly");

        // Duyệt qua tất cả các object trong scene
        foreach (GameObject obj in allObjectsInScene)
        {
            PhotonView photonView = obj.GetComponent<PhotonView>();

            // Nếu object có PhotonView và nó không phải là object của client này
            if (photonView != null && !photonView.IsMine)
            {
                // Chuyển quyền sở hữu của object cho client hiện tại
                photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
                Debug.Log("Đã chuyển quyền sở hữu cho máy khách");
            }
        }
    }

    // Đảm bảo gọi lại khi có người chơi mới vào phòng
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        // Master Client sẽ gọi lại RPC khi có người chơi mới vào phòng
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("TransferOwnershipToAllClients", RpcTarget.All);
        }
    }
}
