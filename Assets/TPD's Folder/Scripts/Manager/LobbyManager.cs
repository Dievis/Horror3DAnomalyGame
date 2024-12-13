using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField usernameInput; // Input field để nhập username
    public TMP_Text buttonText; // Hiển thị trạng thái kết nối
    public GameObject lobbyPanel; // Panel hiển thị Lobby
    public GameObject roomPanel; // Panel khi người dùng vào phòng
    public TMP_Text roomName; // Tên phòng hiện tại

    public RoomItemUI roomItemPrefab;
    List<RoomItemUI> roomItemsList = new List<RoomItemUI>();
    public Transform contentObject;

    public float timeBetweenUpdates = 1.5f;
    float nextUpdateTime;

    public List<PlayerItemUI> playerItemsList = new List<PlayerItemUI>();
    public PlayerItemUI playerItemPrefab;
    public Transform playerItemParent;

    public GameObject playButton;
    public GameObject connectPanel; // Panel kết nối
    public GameObject loadingPanel; // Panel kết nối

    public void Start()
    {
        connectPanel.SetActive(true); // Hiển thị panel kết nối
        lobbyPanel.SetActive(false);
        loadingPanel.SetActive(false);  // Ẩn Lobby panel khi chưa kết nối
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Khi bấm nút kết nối
    public void OnClickConnect()
    {
        if (usernameInput.text.Length >= 1)
        {
            PhotonNetwork.NickName = usernameInput.text;
            //buttonText.text = "Đang kết nối...";

            // Kiểm tra xem Photon có đang kết nối hay không
            if (!PhotonNetwork.IsConnected)
            {
                // Nếu chưa kết nối, thực hiện kết nối với Photon
                PhotonNetwork.ConnectUsingSettings();
                buttonText.text = "Đang kết nối...";
                Debug.Log("Đang kết nối lại với Photon...");
            }
            else
            {
                // Nếu đã kết nối rồi, thông báo người dùng
                Debug.LogWarning("Photon đã kết nối sẵn, không cần kết nối lại.");
            }
        }
    }


    // Khi kết nối thành công
    public override void OnConnectedToMaster()
    {
        connectPanel.SetActive(false); // Ẩn panel kết nối khi kết nối thành công
        lobbyPanel.SetActive(true); // Hiển thị Lobby panel sau khi kết nối
        PhotonNetwork.JoinLobby(); // Vào lobby của Photon
    }

    // Tạo phòng mới
    public void OnClickCreate()
    {
        if (usernameInput.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(usernameInput.text, new RoomOptions() { MaxPlayers = 4 });
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
        foreach (RoomItemUI item in roomItemsList)
        {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        foreach (RoomInfo room in list)
        {
            RoomItemUI newRoom = Instantiate(roomItemPrefab, contentObject);
            newRoom.SetRoomName(room.Name);
            roomItemsList.Add(newRoom);
        }
    }

    public void OnClickRefreshRooms()
    {
        PhotonNetwork.JoinLobby();   // Gia nhập lại lobby để cập nhật danh sách phòng
    }


    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
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
        foreach (PlayerItemUI item in playerItemsList)
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
            PlayerItemUI newPlayerItem = Instantiate(playerItemPrefab, playerItemParent);
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
        // Hiển thị panel loading khi bắt đầu chơi
        loadingPanel.SetActive(true);

        // Chuyển tới scene của trò chơi (Photon sẽ xử lý việc tải scene)
        PhotonNetwork.LoadLevel("MultiplayerScene");
    }

    public void OnClickBackToMenu()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }

        Loader.Load(Loader.Scene.LoadingScene, Loader.Scene.MainMenuScene);

    }
}
