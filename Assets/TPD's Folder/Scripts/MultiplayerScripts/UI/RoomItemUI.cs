using TMPro;
using UnityEngine;

public class RoomItemUI : MonoBehaviour
{
    public TMP_Text roomNameText;
    private LobbyManager manager;

    public void SetRoomInfo(string roomName, LobbyManager lobbyManager)
    {
        if (roomNameText != null)
        {
            roomNameText.text = roomName;
        }
        else
        {
            Debug.LogError("roomNameText is not assigned in RoomItemUI.");
        }

        this.manager = lobbyManager;
    }

    public void OnClickItem()
    {
        if (manager != null)
        {
            manager.JoinRoom(roomNameText.text);
        }
        else
        {
            Debug.LogError("LobbyManager reference is missing in RoomItemUI.");
        }
    }
}
