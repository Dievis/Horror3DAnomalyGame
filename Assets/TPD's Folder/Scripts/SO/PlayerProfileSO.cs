using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProfileSO", menuName = "Multiplayer/PlayerProfile")]
public class PlayerProfileSO : ScriptableObject
{
    public string playerName;
    public GameRoomSO currentRoom;  // Phòng mà người chơi tham gia

    public void SetPlayerName(string name)
    {
        playerName = name;
    }
}
