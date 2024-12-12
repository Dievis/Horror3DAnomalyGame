


using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviourPunCallbacks
{
    public GameObject pauseMenuUI;
    public GameObject settingsPanelUI;
    private GameObject playerHUD;
    private bool isPaused = false;
    private bool isLeavingRoom = false;

    void Start()
    {
        if (!photonView.IsMine)
        {
            pauseMenuUI.SetActive(false);
            settingsPanelUI.SetActive(false);
            return;
        }

        pauseMenuUI.SetActive(false);
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
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused && settingsPanelUI.activeSelf)
            {
                BackToPauseMenu();
            }
            else if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        if (!photonView.IsMine || isLeavingRoom) return;

        pauseMenuUI.SetActive(false);
        settingsPanelUI.SetActive(false);
        if (playerHUD != null) playerHUD.SetActive(true);
        Time.timeScale = 1f;
        LockCursor();
        isPaused = false;
    }

    public void Pause()
    {
        if (!photonView.IsMine || isLeavingRoom) return;

        pauseMenuUI.SetActive(true);
        settingsPanelUI.SetActive(false);
        if (playerHUD != null) playerHUD.SetActive(false);
        Time.timeScale = 0f;
        UnlockCursor();
        isPaused = true;
    }

    public void OpenSettings()
    {
        if (!photonView.IsMine || isLeavingRoom) return;

        pauseMenuUI.SetActive(false);
        settingsPanelUI.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        if (!photonView.IsMine) return;

        settingsPanelUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void OnClickBackToMenu()
    {
        if (!photonView.IsMine || isLeavingRoom) return;

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

    public void SetMenuButtonsInteractable(bool isInteractable)
    {
        foreach (var button in pauseMenuUI.GetComponentsInChildren<UnityEngine.UI.Button>())
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
        if (!photonView.IsMine) return;

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
