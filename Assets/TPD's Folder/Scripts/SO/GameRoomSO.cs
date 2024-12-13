using UnityEngine;

[CreateAssetMenu(fileName = "New Room", menuName = "Multiplayer/Room")]
public class GameRoomSO : ScriptableObject
{
    public string roomName;  // Tên phòng
    public int maxPlayers = 4;  // Số người tối đa trong phòng
    public int currentPlayers = 0;  // Số người hiện tại trong phòng
    public bool isRoomActive = true;  // Trạng thái của phòng (còn hoạt động hay không)

    public bool CanJoin()
    {
        return currentPlayers < maxPlayers && isRoomActive;
    }

    public void JoinRoom()
    {
        currentPlayers++;
    }

    public void LeaveRoom()
    {
        if (currentPlayers > 0)
        {
            currentPlayers--;
        }
    }

    public void StartGame()
    {
        isRoomActive = false;  // Khóa phòng khi game bắt đầu
    }
}
