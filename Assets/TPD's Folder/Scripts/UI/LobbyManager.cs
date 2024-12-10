using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("UI Elements")]
    public GameObject connectPanel; // Panel kết nối
    public TMP_InputField usernameInput; // Input field để nhập username
    public TMP_Text buttonText; // Hiển thị trạng thái kết nối
    
    public GameObject lobbyPanel; // Panel hiển thị Lobby

    public GameObject roomPanel; // Panel khi người dùng vào phòng
    public TMP_InputField roomInputField; // Input field để nhập tên phòng
    public TMP_Text roomName; // Tên phòng hiện tại

    public GameObject playButton;

    [Header("Prefab")]
    public RoomItem roomItemPrefab;
    List<RoomItem> roomItemsList = new List<RoomItem>();
    public Transform contentObject;

    public float timeBetweenUpdates = 1.5f;
    float nextUpdateTime;

    public List<PlayerItem> playerItemsList = new List<PlayerItem>();
    public PlayerItem playerItemPrefab;
    public Transform playerItemParent;

    
    

    public void Start()
    {
        connectPanel.SetActive(true); // Hiển thị panel kết nối
        lobbyPanel.SetActive(false); // Ẩn Lobby panel khi chưa kết nối
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Khi bấm nút kết nối
    public void OnClickConnect()
    {
        if (usernameInput.text.Length >= 1)
        {
            PhotonNetwork.NickName = usernameInput.text;
            buttonText.text = "Đang kết nối...";
            PhotonNetwork.ConnectUsingSettings(); // Kết nối với Photon
        }
    }

    // Khi kết nối thành công
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server!");
        PhotonNetwork.JoinLobby(); // Đảm bảo tham gia Lobby sau khi kết nối
        connectPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public void OnClickCreate()
    {
        if (roomInputField.text.Length >= 1) // Kiểm tra nếu tên phòng không rỗng
        {
            string roomName = roomInputField.text; // Lấy tên phòng từ trường nhập liệu
            PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 4 }); // Tạo phòng mới với tên đúng
        }
        else
        {
            Debug.LogWarning("Vui lòng nhập tên phòng.");
        }
    }

    public void KickPlayer(Player playerToKick)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CloseConnection(playerToKick);
        }
        else
        {
            Debug.LogWarning("Only the Master Client can kick players.");
        }
    }

    // Khi vào phòng
    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = "Phòng: " + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

    // Cập nhật danh sách phòng
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (Time.time >= nextUpdateTime)
        {
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + timeBetweenUpdates;
        }
    }

    void UpdateRoomList(List<RoomInfo> list)
    {
        foreach (RoomItem item in roomItemsList)
        {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        foreach (RoomInfo room in list)
        {
            RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
            newRoom.SetRoomName(room.Name);
            roomItemsList.Add(newRoom);
        }
    }

    public void JoinRoom(string roomName)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            Debug.LogError($"Cannot join room. Client is not ready. Current state: {PhotonNetwork.NetworkClientState}");
        }
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    // Khi rời khỏi phòng
    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    // Cập nhật danh sách người chơi
    void UpdatePlayerList()
    {
        foreach (PlayerItem item in playerItemsList)
        {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();

        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerItemParent);
            newPlayerItem.SetPlayerInfo(player.Value);

            playerItemsList.Add(newPlayerItem);
        }
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        UpdatePlayerList();
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 1)
        {
            playButton.SetActive(true);
        }
        else
        {
            playButton.SetActive(false);
        }
    }

    public void OnClickPlayButton()
    {
        // Chuyển đến LoadingScene trước khi vào game
        Loader.Load(Loader.Scene.LoadingScene, Loader.Scene.Game);
    }

    public void OnClickBackToMenu()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }

        Loader.Load(Loader.Scene.LoadingScene, Loader.Scene.MainMenuScene);
    }

    // Hàm mới cho chức năng refresh
    public void OnClickRefresh()
    {
        PhotonNetwork.JoinLobby(); // Để lấy lại danh sách phòng
    }



}
