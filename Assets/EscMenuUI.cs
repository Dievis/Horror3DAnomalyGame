using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscMenuUI : MonoBehaviourPunCallbacks
{
    public GameObject escMenuUI;
    public GameObject settingsPanelUI;
    private GameObject playerHUD;
    private bool isPaused = false;
    private bool isLeavingRoom = false;

    void Start()
    {
        // Kiểm tra nếu đây là đối tượng của người chơi mình
        if (!photonView.IsMine)
        {
            escMenuUI.SetActive(false);
            settingsPanelUI.SetActive(false);
            return;
        }

        escMenuUI.SetActive(false);
        settingsPanelUI.SetActive(false);
        FindPlayerHUD();

        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Đang chạy trên chế độ multiplayer");
        }
        else
        {
            Debug.Log("Đang chạy trên chế độ singleplayer");
        }
    }

    void Update()
    {
        // Chỉ thực thi khi nhấn phím ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused && settingsPanelUI.activeSelf)
            {
                BackToPauseMenu();
            }
            else if (isPaused)
            {
                CloseEscMenu();
            }
            else
            {
                OpenEscMenu();
            }
        }
    }

    // Hàm mở menu esc (chỉ cho client hiện tại)
    public void OpenEscMenu()
    {
        if (isLeavingRoom) return; // Nếu đang rời phòng, không làm gì

        if (photonView.IsMine) // Chỉ mở menu cho client này
        {
            isPaused = true;
            escMenuUI.SetActive(true);
            settingsPanelUI.SetActive(false);
            if (playerHUD != null) playerHUD.SetActive(false);
            UnlockCursor();
        }
    }

    // Hàm đóng menu esc (chỉ cho client hiện tại)
    public void CloseEscMenu()
    {
        if (isLeavingRoom) return; // Nếu đang rời phòng, không làm gì

        if (photonView.IsMine) // Chỉ đóng menu cho client này
        {
            isPaused = false;
            escMenuUI.SetActive(false);
            if (playerHUD != null) playerHUD.SetActive(true);
            LockCursor();
        }
    }

    // Mở settings (chỉ cho client hiện tại)
    public void OpenSettings()
    {
        if (isLeavingRoom) return; // Nếu đang rời phòng, không làm gì

        if (photonView.IsMine) // Chỉ mở settings cho client này
        {
            escMenuUI.SetActive(false);
            settingsPanelUI.SetActive(true);
        }
    }

    // Quay lại menu esc từ settings (chỉ cho client hiện tại)
    public void BackToPauseMenu()
    {
        if (photonView.IsMine) // Chỉ cho client hiện tại
        {
            settingsPanelUI.SetActive(false);
            escMenuUI.SetActive(true);
        }
    }

    // Khi nhấn nút "Back to menu" (rời phòng hoặc chuyển đến menu chính)
    public void OnClickBackToMenu()
    {
        if (isLeavingRoom) return; // Nếu đang rời phòng, không làm gì

        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                Debug.Log("Leaving room...");
                isLeavingRoom = true;
                SetMenuButtonsInteractable(false);
                SetRoomPropertiesBeforeLeaving(); // Gọi SetProperties trước khi rời phòng
                LeaveRoom(); // Rời khỏi phòng ngay lập tức
                StartCoroutine(WaitForLeftRoom());
            }
            else
            {
                Debug.LogWarning("Photon client chưa sẵn sàng để rời phòng.");
            }
        }
        else
        {
            Debug.Log("Đang chuyển trực tiếp đến Main Menu...");
            LoadMainMenuScene();
        }
    }

    private void SetRoomPropertiesBeforeLeaving()
    {
        if (PhotonNetwork.InRoom)
        {
            // Thực hiện thay đổi các thuộc tính phòng (SetProperties) trước khi rời phòng
            ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
            properties.Add("GamePaused", true); // Ví dụ thêm một thuộc tính "GamePaused"
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
            Debug.Log("Đã cập nhật thuộc tính phòng trước khi rời phòng.");
        }
    }

    private void LeaveRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
    }

    private IEnumerator WaitForLeftRoom()
    {
        float timeout = 5f; // Thời gian chờ tối đa
        float elapsed = 0f;

        while (PhotonNetwork.InRoom && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!PhotonNetwork.InRoom)
        {
            Debug.Log("Successfully left room.");
            LoadMainMenuScene(); // Sau khi đã rời phòng, mới chuyển cảnh
        }
        else
        {
            Debug.LogWarning("Timeout while waiting to leave room.");
            LoadMainMenuScene(); // Nếu bị timeout, chuyển cảnh về MainMenu
        }
    }

    private void LoadMainMenuScene()
    {
        // Đảm bảo client không còn kết nối với Photon khi chuyển cảnh
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        SceneManager.LoadScene("MainMenuScene");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"Đã bị ngắt kết nối: {cause}. Chuyển về Main Menu.");
        LoadMainMenuScene(); // Dự phòng trong trường hợp bị ngắt kết nối
    }

    // Thay đổi trạng thái của các nút menu
    public void SetMenuButtonsInteractable(bool isInteractable)
    {
        foreach (var button in escMenuUI.GetComponentsInChildren<UnityEngine.UI.Button>())
        {
            button.interactable = isInteractable;
        }

        foreach (var button in settingsPanelUI.GetComponentsInChildren<UnityEngine.UI.Button>())
        {
            button.interactable = isInteractable;
        }
    }

    private void FindPlayerHUD()
    {
        playerHUD = GameObject.FindGameObjectWithTag("PlayerHUD");
        if (playerHUD == null)
        {
            Debug.LogWarning("Player HUD không tìm thấy! Hãy đảm bảo là HUD của player đã có tag và player đã spawn.");
        }
        else
        {
            Debug.Log("Player HUD đã tìm thấy và gắn thành công.");
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
