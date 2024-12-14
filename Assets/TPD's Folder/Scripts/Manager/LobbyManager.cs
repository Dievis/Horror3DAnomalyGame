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

    [Header("Update Settings")]
    public float timeBetweenUpdates = 1.5f;
    private float nextUpdateTime;

    [Header("Player Management")]
    public List<PlayerItemUI> playerItemsList = new List<PlayerItemUI>();
    public PlayerItemUI playerItemPrefab;
    public Transform playerItemParent;

    [Header("Gameplay UI")]
    public GameObject playButton;

    private bool canPlay = false;

    public void Start()
    {
        connectPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        loadingPanel.SetActive(false);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Khi bấm nút kết nối
    public void OnClickConnect()
    {
        if (usernameInput.text.Length >= 1)
        {
            PhotonNetwork.NickName = usernameInput.text;
            if (!PhotonNetwork.IsConnected)
            {
                buttonText.text = "Đang kết nối...";
                PhotonNetwork.ConnectUsingSettings();
                Debug.Log("Đang kết nối lại với Photon...");
            }
            else
            {
                connectPanel.SetActive(false);
                lobbyPanel.SetActive(true);
                PhotonNetwork.JoinLobby();
                Debug.LogWarning("Photon đã kết nối sẵn.");
            }
        }
        else
        {
            Debug.LogWarning("Vui lòng nhập tên người dùng.");
        }
    }

    // Khi kết nối thành công
    public override void OnConnectedToMaster()
    {
        connectPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        PhotonNetwork.JoinLobby();
    }

    // Tạo phòng mới
    public void OnClickCreate()
    {
        if (roomnameInput.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(roomnameInput.text, new RoomOptions() { MaxPlayers = 4 });
        }
        else
        {
            Debug.LogError("Please enter a room name.");
        }
    }

    // Khi vào phòng
    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = "Phòng: " + PhotonNetwork.CurrentRoom.Name;

        // Hiển thị số người chơi hiện tại và tối đa trong phòng
        int currentPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        int maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;
        maxPlayersText.text = "Số người chơi: " + currentPlayers + "/" + maxPlayers;

        // In ra thông báo Master Client
        Debug.Log("Master Client: " + PhotonNetwork.MasterClient.NickName);

        UpdatePlayerList();
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
            if (newPlayerItem != null)
            {
                newPlayerItem.SetPlayerInfo(player.Value);
                playerItemsList.Add(newPlayerItem);
                // Gọi UpdateReadyState trên đối tượng PlayerItemUI vừa tạo
                newPlayerItem.UpdateReadyState();
            }
            else
            {
                Debug.LogError("PlayerItemUI prefab is not assigned.");
            }
        }
    }



    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
        CheckCanPlay();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        UpdatePlayerList(); // Cập nhật lại danh sách người chơi
    }


    private void CheckCanPlay()
    {
        // Đảm bảo chỉ có MasterClient mới có thể thấy nút Play khi tất cả người chơi đã sẵn sàng
        bool allReady = true;
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if (!player.CustomProperties.ContainsKey("IsReady") || !(bool)player.CustomProperties["IsReady"])
            {
                allReady = false;
                break;
            }
        }

        // Chỉ hiển thị nút Play khi là master client và tất cả người chơi đã sẵn sàng
        if (PhotonNetwork.IsMasterClient && allReady)
        {
            playButton.SetActive(true);
        }
        else
        {
            playButton.SetActive(false);
        }
    }


    private void OnClickReadyButton()
    {
        bool isReady = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("IsReady") &&
                       (bool)PhotonNetwork.LocalPlayer.CustomProperties["IsReady"];

        // Gửi trạng thái mới lên Photon
        ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable
    {
        { "IsReady", !isReady }
    };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

        // Cập nhật trạng thái UI cho tất cả các client
        UpdatePlayerReadyState();
    }

    void UpdatePlayerReadyState()
    {
        foreach (PlayerItemUI item in playerItemsList)
        {
            item.UpdateReadyState();  // Gọi phương thức UpdateReadyState() của từng PlayerItemUI
        }
    }


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("IsReady"))
        {
            UpdatePlayerList();  // Cập nhật lại danh sách người chơi để hiển thị trạng thái mới
            CheckCanPlay();  // Kiểm tra lại điều kiện để xem nút Play có cần kích hoạt hay không
        }
    }


    public void OnClickPlayButton()
    {
        loadingPanel.SetActive(true);
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
        PhotonNetwork.JoinLobby();  // Làm mới danh sách phòng khi rời khỏi
    }


    public void OnClickBackToMenu()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        Loader.Load(Loader.Scene.LoadingScene, Loader.Scene.MainMenuScene);
    }

    // Khi bấm nút làm mới danh sách phòng
    public void OnClickRefreshRooms()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();  // Gọi lại JoinLobby để làm mới danh sách phòng
        }
        else
        {
            Debug.LogWarning("Không trong sảnh, không thể làm mới ds phòng.");
        }
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
        PhotonNetwork.JoinRoom(roomName);
    }
}
