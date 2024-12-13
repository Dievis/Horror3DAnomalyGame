using UnityEngine;

[CreateAssetMenu(fileName = "LobbyManagerSO", menuName = "Multiplayer/LobbyManager")]
public class LobbyManagerSO : ScriptableObject
{
    public float timeBetweenUpdates = 1.5f;  // Thời gian giữa các lần cập nhật danh sách phòng
    public float nextUpdateTime;  // Thời gian cập nhật tiếp theo

    public void UpdateNextUpdateTime()
    {
        nextUpdateTime = Time.time + timeBetweenUpdates;
    }
}
