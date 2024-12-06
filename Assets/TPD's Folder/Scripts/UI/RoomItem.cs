using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    public TMP_Text roomName;
    LobbyManager manager;

    private void Start()
    {
        manager = FindObjectOfType<LobbyManager>();
    }

    public void SetRoomName(string _roomName)
    {
        if (roomName != null) { roomName.text = _roomName; }
        else
        {
            Debug.LogError("roomName is not assigned!");
        }
    }

    public void OnClickItem()
    {
        manager.JoinRoom(roomName.text);
    }
}
