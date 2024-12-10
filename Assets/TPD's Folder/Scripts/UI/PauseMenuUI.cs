using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PauseMenuUI : MonoBehaviourPunCallbacks
{
    public GameObject pauseMenuUI;     // Gán UI menu tạm dừng từ Editor
    public GameObject settingsPanelUI; // Gán UI panel cài đặt từ Editor
    private GameObject playerHUD;      // Tự động tìm HUD khi player spawn
    private bool isPaused = false;

    void Start()
    {
        if (!photonView.IsMine)
        {
            // Nếu không phải player local, tắt PauseMenu để tránh xung đột
            pauseMenuUI.SetActive(false);
            settingsPanelUI.SetActive(false);
            return;
        }

        pauseMenuUI.SetActive(false);    // Ẩn menu tạm dừng ban đầu
        settingsPanelUI.SetActive(false); // Ẩn settings panel ban đầu
        //UnlockCursor();                  // Mở khóa con trỏ khi khởi động game
        FindPlayerHUD();                 // Tự động tìm HUD

        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Running in multiplayer mode");
        }
        else
        {
            Debug.Log("Running in singleplayer mode");
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return; // Chỉ xử lý cho local player

        // Kiểm tra nếu người chơi nhấn phím Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused && settingsPanelUI.activeSelf)
            {
                BackToPauseMenu(); // Nếu đang ở setting panel thì quay lại pause menu
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

    // Hàm tiếp tục game
    public void Resume()
    {
        if (!photonView.IsMine) return; // Chỉ xử lý cho local player

        pauseMenuUI.SetActive(false);      // Ẩn menu tạm dừng
        settingsPanelUI.SetActive(false); // Ẩn panel cài đặt
        if (playerHUD != null) playerHUD.SetActive(true); // Hiển thị lại HUD của player
        Time.timeScale = 1f;              // Khôi phục thời gian game
        LockCursor();                     // Khóa con trỏ chuột
        isPaused = false;                 // Đặt trạng thái là không tạm dừng
    }

    // Hàm tạm dừng game
    public void Pause()
    {
        if (!photonView.IsMine) return; // Chỉ xử lý cho local player

        pauseMenuUI.SetActive(true);      // Hiển thị menu tạm dừng
        settingsPanelUI.SetActive(false); // Ẩn panel cài đặt
        if (playerHUD != null) playerHUD.SetActive(false); // Ẩn HUD của player
        Time.timeScale = 0f;              // Dừng thời gian game
        UnlockCursor();                   // Mở khóa con trỏ chuột
        isPaused = true;                  // Đặt trạng thái là tạm dừng
    }

    public void OpenSettings()
    {
        if (!photonView.IsMine) return; // Chỉ xử lý cho local player

        Debug.Log("Opening settings panel"); // Thông báo kiểm tra
        pauseMenuUI.SetActive(false);        // Ẩn menu tạm dừng
        settingsPanelUI.SetActive(true);     // Hiển thị panel cài đặt
    }

    public void BackToPauseMenu()
    {
        if (!photonView.IsMine) return; // Chỉ xử lý cho local player

        Debug.Log("Returning to pause menu"); // Thông báo kiểm tra
        settingsPanelUI.SetActive(false);     // Ẩn panel cài đặt
        pauseMenuUI.SetActive(true);          // Hiển thị menu tạm dừng
    }

    // Hàm về main menu (gán vào nút trong UI)
    public void BackToMainMenu()
    {
        if (!photonView.IsMine) return; // Chỉ xử lý cho local player

        // Kiểm tra nếu đang ở trong phòng
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom(); // Rời phòng multiplayer
        }

        // Chuyển về main menu
        Loader.Load(Loader.Scene.LoadingScene, Loader.Scene.MainMenuScene);
    }


    // Hàm tự động tìm HUD trong player prefab
    private void FindPlayerHUD()
    {
        if (!photonView.IsMine) return;

        // Tìm HUD bằng tag hoặc component (đảm bảo HUD có tag "PlayerHUD" trong prefab)
        playerHUD = GameObject.FindGameObjectWithTag("PlayerHUD");
        if (playerHUD == null)
        {
            Debug.LogWarning("Player HUD not found! Make sure it has the correct tag or is spawned.");
        }
        else
        {
            Debug.Log("Player HUD found and assigned.");
        }
    }

    // Hàm khóa con trỏ chuột
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; // Khóa con trỏ vào giữa màn hình
        Cursor.visible = false;                  // Ẩn con trỏ
    }

    // Hàm mở khóa con trỏ chuột
    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;  // Mở khóa con trỏ
        Cursor.visible = true;                   // Hiển thị con trỏ
    }
}
