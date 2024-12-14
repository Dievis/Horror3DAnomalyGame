using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("UI Elements - Connection")]
    public TMP_InputField usernameInput;
    public TMP_Text buttonText;
    public GameObject connectPanel;
    public GameObject loadingPanel;

    [Header("UI Elements - Lobby")]
    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public TMP_InputField roomnameInput;
    public TMP_Text maxPlayersText;
    public TMP_Text roomName;

    [Header("Room Management")]
    public RoomItemUI roomItemPrefab;
    private List<RoomItemUI> roomItemsList = new List<RoomItemUI>();
    public Transform contentObject;

    [Header("Player Management")]
    public List<PlayerItemUI> playerItemsList = new List<PlayerItemUI>();
    public PlayerItemUI playerItemPrefab;
    public Transform playerItemParent;

    [Header("Gameplay UI")]
    public GameObject playButton;

    private void Start()
    {
        connectPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        loadingPanel.SetActive(false);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void OnClickConnect()
    {
        if (usernameInput.text.Length >= 1)
        {
            PhotonNetwork.NickName = usernameInput.text;
            if (!PhotonNetwork.IsConnected)
            {
                buttonText.text = "Đang kết nối...";
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                connectPanel.SetActive(false);
                lobbyPanel.SetActive(true);
                PhotonNetwork.JoinLobby();
            }
        }
    }

    public void OnClickBackToMenu()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        Loader.Load(Loader.Scene.LoadingScene, Loader.Scene.MainMenuScene);
    }

    public override void OnConnectedToMaster()
    {
        connectPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        PhotonNetwork.JoinLobby(); // Gia nhập lobby sau khi kết nối với Master
    }

    public void OnClickCreate()
    {
        if (roomnameInput.text.Length >= 1)
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.CreateRoom(roomnameInput.text, new RoomOptions() { MaxPlayers = 4 });
            }
            else
            {
                Debug.LogError("Photon is not connected and ready. Waiting for connection.");
            }
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Create room failed. Error: " + message);
        // Có thể thêm thông báo cho người chơi biết phòng không thể tạo được
    }

    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = "Phòng: " + PhotonNetwork.CurrentRoom.Name;

        UpdatePlayerList();
        UpdatePlayerCount();
        UpdatePlayButtonVisibility();

        // Đảm bảo rằng các client khác cũng nhận được thông tin về người chơi mới
        PhotonNetwork.CurrentRoom.IsVisible = true; // Đảm bảo phòng là visible và có thể join
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Gọi lại để cập nhật danh sách người chơi khi có người mới vào
        UpdatePlayerList();
        UpdatePlayerCount();
        UpdatePlayButtonVisibility(); // Cập nhật lại nút Play khi có player vào
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Gọi lại để cập nhật danh sách người chơi khi có player rời đi
        UpdatePlayerList();
        UpdatePlayerCount();
        UpdatePlayButtonVisibility(); // Cập nhật lại nút Play khi có player rời đi
    }

    void UpdatePlayerList()
    {
        // Đảm bảo danh sách người chơi được cập nhật chính xác
        foreach (PlayerItemUI item in playerItemsList)
        {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();

        foreach (KeyValuePair<int, Player> kvp in PhotonNetwork.CurrentRoom.Players)
        {
            Player player = kvp.Value;
            if (player == null)
            {
                Debug.LogError("Player in the room is null.");
                continue;
            }

            PlayerItemUI newPlayerItem = Instantiate(playerItemPrefab, playerItemParent);
            newPlayerItem.SetPlayerInfo(player); // Đảm bảo SetPlayerInfo không bị lỗi
            playerItemsList.Add(newPlayerItem);
        }
    }

    void UpdatePlayerCount()
    {
        if (maxPlayersText != null && PhotonNetwork.CurrentRoom != null)
        {
            maxPlayersText.text = "Số người chơi: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
        }
    }

    // Cập nhật trạng thái hiển thị nút Play
    void UpdatePlayButtonVisibility()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Hiển thị nút Play cho Master Client
            playButton.SetActive(true);
        }
        else
        {
            // Ẩn nút Play cho các người chơi không phải Master Client
            playButton.SetActive(false);
        }
    }

    public void OnClickPlayButton()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    public void OnClickLeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomItemUI item in roomItemsList)
        {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        foreach (RoomInfo room in roomList)
        {
            if (!room.RemovedFromList)
            {
                RoomItemUI newRoom = Instantiate(roomItemPrefab, contentObject);
                newRoom.SetRoomInfo(room.Name, this);
                roomItemsList.Add(newRoom);
            }
        }
    }

    public void JoinRoom(string roomName)
    {
        if (!string.IsNullOrEmpty(roomName))
        {
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            Debug.LogError("Tên phòng không hợp lệ để tham gia.");
        }
    }
}
