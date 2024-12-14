using TMPro;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PlayerItemUI : MonoBehaviour
{
    public TMP_Text playerName;

    private Player player;

    public void SetPlayerInfo(Player _player)
    {
        if (_player == null)
        {
            Debug.LogError("Player passed to SetPlayerInfo is null.");
            return;
        }

        player = _player;

        if (playerName != null)
        {
            playerName.text = player.NickName;
        }

        // Remove any ready state functionality, no longer needed
    }
}
