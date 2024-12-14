using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SPauseMenuUI : MonoBehaviourPunCallbacks // Kế thừa MonoBehaviourPunCallbacks để xử lý các callback Photon
{
    public GameObject pauseMenuUI;
    public GameObject settingsPanelUI;
    private GameObject playerHUD;
    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        settingsPanelUI.SetActive(false);
        FindPlayerHUD();
    }

    void Update()
    {
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
        pauseMenuUI.SetActive(false);
        settingsPanelUI.SetActive(false);
        if (playerHUD != null) playerHUD.SetActive(true);
        Time.timeScale = 1f;
        LockCursor();
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        settingsPanelUI.SetActive(false);
        if (playerHUD != null) playerHUD.SetActive(false);
        Time.timeScale = 0f;
        UnlockCursor();
        isPaused = true;
    }

    public void OpenSettings()
    {
        pauseMenuUI.SetActive(false);
        settingsPanelUI.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        settingsPanelUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void OnClickBackToMenu()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect(); // Ngắt kết nối Photon trước khi chuyển sang menu chính
        }
        Debug.Log("Đã ngắt kết nối photon thành công");
        Loader.Load(Loader.Scene.LoadingScene, Loader.Scene.MainMenuScene);
    }

    private void LoadMainMenuScene()
    {
        SceneManager.LoadScene("MainMenuScene");
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

    // Ghi đè OnConnectedToMaster để kiểm tra kết nối lại
    public override void OnConnectedToMaster()
    {
        Debug.Log("Đã kết nối lại với Photon master server.");
        PhotonNetwork.JoinLobby(); // Vào lại lobby sau khi kết nối thành công
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Đã vào lobby thành công.");
    }

    // Phương thức xử lý khi kết nối lại thất bại
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("Đã bị ngắt kết nối: " + cause.ToString());
        // Có thể yêu cầu người chơi quay lại menu chính hoặc thử lại
    }
}
