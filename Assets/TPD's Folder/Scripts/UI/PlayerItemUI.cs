using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class PlayerItemUI : MonoBehaviour
{
    public TMP_Text playerName;
    private Player player;

    public TMP_Text readyStateText; // Hiển thị trạng thái "Sẵn sàng" hoặc "Hủy sẵn sàng"
    public Button readyButton;

    public void SetPlayerInfo(Player _player)
    {
        if (_player == null)
        {
            Debug.LogError("Player is null.");
            return;
        }

        player = _player;

        // Kiểm tra trước khi cập nhật playerName
        if (playerName != null)
        {
            playerName.text = _player.NickName;
        }
        else
        {
            Debug.LogError("playerName is not assigned!");
        }

        UpdateReadyState(); // Cập nhật trạng thái ban đầu của nút và text
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(OnClickReadyButton);
    }

    public void UpdateReadyState()
    {
        if (readyStateText != null && readyButton != null)
        {
            // Kiểm tra và cập nhật trạng thái
            if (player.CustomProperties.ContainsKey("IsReady") && (bool)player.CustomProperties["IsReady"])
            {
                readyStateText.text = "Hủy sẵn sàng";
                readyButton.GetComponentInChildren<TMP_Text>().text = "Hủy sẵn sàng";
            }
            else
            {
                readyStateText.text = "Sẵn sàng";
                readyButton.GetComponentInChildren<TMP_Text>().text = "Sẵn sàng";
            }
        }
    }

    private void OnDestroy()
    {
        if (readyButton != null)
        {
            readyButton.onClick.RemoveListener(OnClickReadyButton);
        }
    }


    private void OnClickReadyButton()
    {
        // Lấy trạng thái hiện tại và đổi trạng thái IsReady
        bool isReady = player.CustomProperties.ContainsKey("IsReady") && (bool)player.CustomProperties["IsReady"];

        // Gửi trạng thái mới lên Photon
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { "IsReady", !isReady }
        });

        UpdateReadyState();
    }

    

}
