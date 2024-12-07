using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuESC : MonoBehaviour
{
    public GameObject pauseMenuUI;    // Menu tạm dừng
    public GameObject optionsMenuUI; // Menu tùy chọn
    public GameObject playerHUD;     // HUD của người chơi

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuUI.activeInHierarchy)
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
        if (playerHUD != null)
        {
            playerHUD.SetActive(true); // Bật lại HUD
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1; // Tiếp tục game
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        if (playerHUD != null)
        {
            playerHUD.SetActive(false); // Tắt HUD khi tạm dừng
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0; // Tạm dừng game
    }

    public void OpenOptions()
    {
        optionsMenuUI.SetActive(true); // Hiển thị menu tùy chọn
    }

    public void ExitToMenu()
    {
        Loader.Load(Loader.Scene.MainMenuScene);
        Time.timeScale = 1; // Đảm bảo rằng game tiếp tục khi quay lại menu chính
    }
}
