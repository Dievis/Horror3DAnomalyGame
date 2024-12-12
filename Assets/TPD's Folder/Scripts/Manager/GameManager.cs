using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using Photon.Pun.Demo.PunBasics;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    private GameObject hostLeftPanel; // Panel hiển thị khi chủ phòng thoát

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} đã rời game.");
        RemovePlayerObjects(otherPlayer);

        // Kiểm tra nếu người chơi rời là chủ phòng
        if (PhotonNetwork.IsMasterClient == false && otherPlayer.IsMasterClient)
        {
            Debug.Log("Chủ phòng đã thoát. Quay lại menu.");

            if (PhotonNetwork.IsConnectedAndReady)
            {
                ShowHostLeftPanel();
            }
            else
            {
                Debug.LogWarning("Photon client chưa sẵn sàng để xử lý việc chủ phòng rời đi.");
            }
        }
    }


    private void RemovePlayerObjects(Player player)
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView view = obj.GetComponent<PhotonView>();
            if (view != null && view.Owner == player)
            {
                PhotonNetwork.Destroy(obj); // Xóa object của người chơi đã thoát
            }
        }
    }

    private void ShowHostLeftPanel()
    {
        if (hostLeftPanel != null) hostLeftPanel.SetActive(true); // Hiển thị thông báo
        StartCoroutine(ReturnToMenuAfterDelay());
    }

    private IEnumerator ReturnToMenuAfterDelay()
    {
        yield return new WaitForSeconds(3); // Đợi 3 giây trước khi quay lại menu

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

    // Xử lý khi client rời phòng (tự rời hoặc mất kết nối)
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Mất kết nối khỏi Photon.");
        Loader.Load(Loader.Scene.LoadingScene, Loader.Scene.MainMenuScene);
    }

    private void FindHostLeftPanel()
    {
        if (!photonView.IsMine) return;

        hostLeftPanel = GameObject.FindGameObjectWithTag("HostLeftPanel");
        if (hostLeftPanel == null)
        {
            Debug.LogWarning("Giao diện không tìm thấy!");
        }
        else
        {
            Debug.Log("Giao diện đã tìm thấy và gắn thành công.");
        }
    }
}