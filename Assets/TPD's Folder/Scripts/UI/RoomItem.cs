using TMPro;
using UnityEngine;

public class RoomItem : MonoBehaviour
{
    public TMP_Text roomName; // Hiển thị tên phòng
    private LobbyManager manager; // Tham chiếu đến LobbyManager

    private void Start()
    {
        manager = FindObjectOfType<LobbyManager>();

        if (manager == null)
        {
            Debug.LogError("LobbyManager not found in the scene!");
        }
    }

    public void SetRoomName(string _roomName)
    {
        if (roomName != null)
        {
            roomName.text = _roomName;
        }
        else
        {
            Debug.LogError("roomName TMP_Text is not assigned!");
        }
    }

    public void OnClickItem()
    {
        if (manager != null && roomName != null)
        {
            // Kiểm tra xem đối tượng này còn tồn tại không
            if (this != null)
            {
                manager.JoinRoom(roomName.text);
            }
            else
            {
                Debug.LogError("RoomItem has been destroyed before OnClickItem was executed.");
            }
        }
        else
        {
            Debug.LogError("Cannot join room: LobbyManager or roomName is null!");
        }
    }
}
