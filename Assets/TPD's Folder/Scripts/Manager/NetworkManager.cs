using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject hostLeftPanel;

    //Lưu trữ đầu tiên là MasterClient
    private Player initialMasterClient;

    private void Start()
    {
        initialMasterClient = PhotonNetwork.MasterClient;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} đã rời game.");

        // Kiểm tra nếu player rời đi là Master Client ban đầu
        if (otherPlayer == initialMasterClient)
        {
            // Gửi RPC yêu cầu tất cả các client ngắt kết nối
            photonView.RPC("HandleMasterClientLeft", RpcTarget.All);
        }

        // Xóa các đối tượng của người chơi đã rời đi
        RemovePlayerObjects(otherPlayer);
    }

    [PunRPC]
    private void HandleMasterClientLeft()
    {
        Debug.Log("Master Client ban đầu đã rời đi, tất cả các client sẽ ngắt kết nối.");
        ShowMasterDisconnectedMessage();
        StartCoroutine(DisconnectAfterDelay());
    }

    private IEnumerator DisconnectAfterDelay()
    {
        yield return new WaitForSeconds(3);
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.LeaveRoom();
            Loader.Load(Loader.Scene.LoadingScene, Loader.Scene.MainMenuScene);
        }
        else
        {
            Debug.LogWarning("Photon client không còn kết nối khi đang rời phòng.");
        }
    }

    public void RemovePlayerObjects(Player player)
    {
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            PhotonView photonView = obj.GetComponent<PhotonView>();
            if (photonView != null && photonView.Owner == player)
            {
                // Chuyển quyền sở hữu cho MasterClient nếu cần
                if (!photonView.IsMine && PhotonNetwork.IsMasterClient)
                {
                    photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
                }

                // Chỉ xóa nếu là chủ sở hữu
                if (photonView.IsMine)
                {
                    PhotonNetwork.Destroy(obj);
                }
            }
        }
    }

    private void ShowMasterDisconnectedMessage()
    {
        Debug.Log("Chủ phòng đã thoát, trở về menu chính...");
        if (hostLeftPanel != null)
        {
            hostLeftPanel.SetActive(true);
        }
    }

    // Xử lý khi Master Client bị chuyển
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (!PhotonNetwork.IsMasterClient && newMasterClient != initialMasterClient)
        {
            Debug.Log("Master Client đã thay đổi, tất cả các client sẽ ngắt kết nối.");
            ShowMasterDisconnectedMessage();
            StartCoroutine(DisconnectAfterDelay());
        }
    }

    // Xử lý khi bị ngắt kết nối
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.Log("Ngắt kết nối: " + cause.ToString());

        if (cause == DisconnectCause.Exception || cause == DisconnectCause.DisconnectByServerLogic)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Master Client đã rời, tất cả các client sẽ ngắt kết nối.");
                ShowMasterDisconnectedMessage();
                StartCoroutine(DisconnectAfterDelay());
            }
        }
        else
        {
            ShowMasterDisconnectedMessage(); // Các lý do ngắt kết nối khác không liên quan đến Master Client
        }
    }
}
