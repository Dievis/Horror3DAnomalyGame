using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscMenuUI : MonoBehaviourPun
{
    public GameObject EscMenuPanel;        // Menu Pause UI
    public GameObject SettingsPanel;   // Settings Panel UI
    private bool isPaused = false;       // Trạng thái pause của player local

    void Start()
    {
        // Ẩn các UI ngay khi bắt đầu
        EscMenuPanel.SetActive(false);
        SettingsPanel.SetActive(false);

        // Nếu đây không phải là Player Local, tắt script này
        if (!photonView.IsMine)
        {
            enabled = false;
        }
    }

    void Update()
    {
        // Chỉ xử lý khi nhấn phím ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused && SettingsPanel.activeSelf)
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

    /// <summary>
    /// Resume game
    /// </summary>
    public void Resume()
    {
        EscMenuPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        LockCursor();  // Khóa con trỏ
        isPaused = false;

        Debug.Log("Game đã resume.");
    }

    /// <summary>
    /// Pause game
    /// </summary>
    public void Pause()
    {
        EscMenuPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        UnlockCursor(); // Mở khóa con trỏ
        isPaused = true;

        Debug.Log("Pause menu đã mở.");
    }

    /// <summary>
    /// Mở settings
    /// </summary>
    public void OpenSettings()
    {
        EscMenuPanel.SetActive(false);
        SettingsPanel.SetActive(true);

        Debug.Log("Đã mở settings menu.");
    }

    /// <summary>
    /// Quay lại menu pause từ settings
    /// </summary>
    public void BackToPauseMenu()
    {
        SettingsPanel.SetActive(false);
        EscMenuPanel.SetActive(true);

        Debug.Log("Quay lại pause menu.");
    }

    /// <summary>
    /// Quay lại menu chính
    /// </summary>
    public void OnClickBackToMenu()
    {
        Debug.Log("Quay lại menu chính.");
        UnlockCursor();  // Mở khóa con trỏ trước khi thoát
        PhotonNetwork.LeaveRoom(); // Thoát khỏi phòng
        Loader.Load(Loader.Scene.LoadingScene, Loader.Scene.MainMenuScene); // Load scene menu chính
    }

    /// <summary>
    /// Khóa con trỏ chuột
    /// </summary>
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Mở khóa con trỏ chuột
    /// </summary>
    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
