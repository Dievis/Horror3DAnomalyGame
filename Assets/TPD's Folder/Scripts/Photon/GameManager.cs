using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} Đã rời game.");
        RemovePlayerObjects(otherPlayer);
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
}
