using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

public class PlayerItem : MonoBehaviour
{
    public TMP_Text playerNameText; // Text để hiển thị tên người chơi

    // Thiết lập thông tin người chơi
    public void SetPlayerInfo(Player _player)
    {
        if (_player == null)
        {
            Debug.LogError("Player is null in SetPlayerInfo!");
            return;
        }

        if (playerNameText != null)
        {
            playerNameText.text = _player.NickName; // Hiển thị tên người chơi
        }
        else
        {
            Debug.LogError("playerNameText is not assigned!");
        }
    }
}
