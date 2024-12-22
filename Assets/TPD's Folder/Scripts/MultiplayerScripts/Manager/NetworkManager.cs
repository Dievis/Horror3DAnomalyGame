using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // Các biến để tham chiếu đến UIManager và GameManager
    private UIManager uiManager; // Quản lý UI của trò chơi
    private GameManager gameManager; // Quản lý logic của trò chơi

    // Biến lưu trữ Master Client ban đầu
    private Player initialMasterClient; // Lưu trữ Player là Master Client ban đầu khi game bắt đầu

    // Hàm bắt đầu, gọi khi scene được khởi tạo
    private void Start()
    {
        // Tìm và gán UIManager và GameManager
        uiManager = FindObjectOfType<UIManager>();
        gameManager = FindObjectOfType<GameManager>();

        // Lưu MasterClient ban đầu
        initialMasterClient = PhotonNetwork.MasterClient;
    }

    // Hàm được gọi khi người chơi bấm nút "Back to Menu"
    public void OnClickBackToMenu()
    {
        // Kiểm tra nếu người chơi là Master Client
        if (PhotonNetwork.IsMasterClient)
        {
            // Gửi RPC yêu cầu tất cả các client ngắt kết nối
            photonView.RPC("HandleMasterClientLeft", RpcTarget.All);
        }
        else
        {
            // Nếu không phải Master Client, cũng gửi RPC để ngắt kết nối tất cả client
            photonView.RPC("HandleMasterClientLeft", RpcTarget.All);
        }
    }

    // Khi một người chơi rời phòng, hàm này sẽ được gọi
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} đã rời game.");

        // Kiểm tra nếu người chơi rời phòng là Master Client ban đầu
        if (otherPlayer == initialMasterClient)
        {
            // Gửi RPC yêu cầu tất cả các client ngắt kết nối khi Master Client rời
            photonView.RPC("HandleMasterClientLeft", RpcTarget.All);
        }

        // Xóa các đối tượng của người chơi đã rời phòng
        RemovePlayerObjects(otherPlayer);
    }

    // RPC sẽ được gọi khi Master Client rời đi
    [PunRPC]
    private void HandleMasterClientLeft()
    {
        Debug.Log("Master Client ban đầu đã rời đi.");

        // Nếu có UIManager, hiển thị thông báo rằng Master Client đã rời
        if (uiManager != null)
        {
            uiManager.ShowHostLeftPanel();
        }

        // Bắt đầu Coroutine để ngắt kết nối sau một khoảng thời gian
        StartCoroutine(DisconnectAfterDelay());
    }

    // Coroutine để ngắt kết nối sau một khoảng thời gian
    private IEnumerator DisconnectAfterDelay()
    {
        yield return new WaitForSeconds(3); // Đợi 3 giây trước khi ngắt kết nối

        // Kiểm tra nếu Photon vẫn kết nối
        if (PhotonNetwork.IsConnectedAndReady)
        {
            // Reset lại UI nếu có UIManager
            if (uiManager != null)
            {
                uiManager.ResetUI();
            }
            // Reset lại trò chơi nếu có GameManager
            if (gameManager != null)
            {
                gameManager.ResetGame();
            }

            // Rời phòng và quay lại màn hình chính
            PhotonNetwork.LeaveRoom();
            Loader.Load(Loader.Scene.LoadingScene, Loader.Scene.MainMenuScene);
        }
        else
        {
            // Cảnh báo nếu không còn kết nối Photon khi ngắt kết nối
            Debug.LogWarning("Photon client không còn kết nối khi đang rời phòng.");
        }
    }

    // Hàm để xóa các đối tượng của người chơi đã rời phòng
    public void RemovePlayerObjects(Player player)
    {
        // Duyệt qua tất cả các đối tượng trong scene
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            PhotonView photonView = obj.GetComponent<PhotonView>();
            if (photonView != null && photonView.Owner == player)
            {
                // Chuyển quyền sở hữu đối tượng cho Master Client nếu cần thiết
                if (!photonView.IsMine && PhotonNetwork.IsMasterClient)
                {
                    photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
                }

                // Chỉ xóa đối tượng nếu nó thuộc sở hữu của người chơi này
                if (photonView.IsMine)
                {
                    PhotonNetwork.Destroy(obj);
                }
            }
        }
    }

    // Khi Master Client thay đổi, hàm này sẽ được gọi
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // Nếu không phải Master Client và Master Client đã thay đổi
        if (!PhotonNetwork.IsMasterClient && newMasterClient != initialMasterClient)
        {
            Debug.Log("Master Client đã thay đổi, tất cả các client sẽ ngắt kết nối.");

            // Reset UI khi Master Client thay đổi
            if (uiManager != null)
            {
                uiManager.ResetUI();
            }

            // Reset game khi Master Client thay đổi
            if (gameManager != null)
            {
                gameManager.ResetGame();
            }

            // Ngắt kết nối sau một khoảng thời gian
            StartCoroutine(DisconnectAfterDelay());
        }
    }

    // Khi bị ngắt kết nối, hàm này sẽ được gọi
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.Log($"Ngắt kết nối: {cause}");

        // Kiểm tra lý do ngắt kết nối
        if (cause == DisconnectCause.Exception || cause == DisconnectCause.DisconnectByServerLogic)
        {
            // Nếu Master Client đã rời, tất cả các client sẽ ngắt kết nối
            Debug.Log("Master Client đã rời, tất cả các client sẽ ngắt kết nối.");

            // Nếu không phải Master Client, hiển thị thông báo và ngắt kết nối
            if (!PhotonNetwork.IsMasterClient)
            {
                uiManager.ShowHostLeftPanel();
                StartCoroutine(DisconnectAfterDelay());
            }
        }
        else
        {
            // Reset UI và Game khi ngắt kết nối vì lý do khác
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
