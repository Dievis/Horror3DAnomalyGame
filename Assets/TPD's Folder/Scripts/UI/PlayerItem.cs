using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class PlayerItem : MonoBehaviour
{
    public TMP_Text playerName;
    public Button kickButton;
    private Player player;

    public void SetPlayerInfo(Player _player)
    {
        player = _player;
        playerName.text = _player.NickName;

        // Chỉ hiển thị nút Kick nếu người chơi hiện tại là MasterClient và người này không phải chính họ
        kickButton.gameObject.SetActive(PhotonNetwork.IsMasterClient && _player != PhotonNetwork.LocalPlayer);
    }

    public void OnClickKickButton()
    {
        // Gọi hàm KickPlayer từ LobbyManager
        FindObjectOfType<LobbyManager>().KickPlayer(player);
    }
}
