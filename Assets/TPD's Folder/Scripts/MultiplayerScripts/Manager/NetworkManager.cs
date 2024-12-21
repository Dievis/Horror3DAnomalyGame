using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private UIManager uiManager;
    private GameManager gameManager;
    //Lưu trữ đầu tiên là MasterClient
    private Player initialMasterClient;

    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        gameManager = FindObjectOfType<GameManager>();
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
        Debug.Log("Master Client ban đầu đã rời đi.");
        if (uiManager != null)
        {
            uiManager.ShowHostLeftPanel(); // Hiển thị hostLeftPanel thông qua UIManager
        }

        StartCoroutine(DisconnectAfterDelay());
    }

    private IEnumerator DisconnectAfterDelay()
    {
        yield return new WaitForSeconds(3);

        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (uiManager != null)
            {
                uiManager.ResetUI(); // Reset UI khi Master Client thay đổi
            }
            if (gameManager != null)
            {
                gameManager.ResetGame(); // Reset game khi rời phòng
            }
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

    // Xử lý khi Master Client bị chuyển
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (!PhotonNetwork.IsMasterClient && newMasterClient != initialMasterClient)
        {
            Debug.Log("Master Client đã thay đổi, tất cả các client sẽ ngắt kết nối.");
            if (uiManager != null)
            {
                uiManager.ResetUI(); // Reset UI khi Master Client thay đổi
            }
            if (gameManager != null)
            {
                gameManager.ResetGame(); // Reset game khi Master Client thay đổi
            }
            StartCoroutine(DisconnectAfterDelay());
        }
    }

    // Xử lý khi bị ngắt kết nối
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.Log($"Ngắt kết nối: {cause}");

        // Kiểm tra lý do ngắt kết nối
        if (cause == DisconnectCause.Exception || cause == DisconnectCause.DisconnectByServerLogic)
        {
            Debug.Log("Master Client đã rời, tất cả các client sẽ ngắt kết nối.");
            if (!PhotonNetwork.IsMasterClient)
            {
                uiManager.ShowHostLeftPanel(); // Hiển thị panel thông qua UIManager
                StartCoroutine(DisconnectAfterDelay());
            }
        }
        else
        {
            if (uiManager != null)
            {
                uiManager.ResetUI(); 
            }
            if (gameManager != null)
            {
                gameManager.ResetGame(); 
            }
        }
    }

}
