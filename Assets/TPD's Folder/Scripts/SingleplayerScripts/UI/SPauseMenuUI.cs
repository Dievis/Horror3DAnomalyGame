using UnityEngine;
using UnityEngine.SceneManagement;

public class SPauseMenuUI : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject settingsPanelUI;
    public GameObject StaminaBar;
    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        settingsPanelUI.SetActive(false);
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
        if (StaminaBar != null) StaminaBar.SetActive(true);
        Time.timeScale = 1f;  // Ensure the game resumes correctly
        LockCursor();
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        settingsPanelUI.SetActive(false);
        if (StaminaBar != null) StaminaBar.SetActive(false);
        Time.timeScale = 0f;  // Pause the game when the menu is active
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
        Debug.Log("Đã quay lại menu chính.");
        Time.timeScale = 1f;  // Ensure time scale is reset when returning to the main menu
        UnlockCursor();  // Make sure cursor is unlocked when returning to menu
        LoadMainMenuScene();
    }

    private void LoadMainMenuScene()
    {
        SceneManager.LoadScene("MainMenuScene"); // Đảm bảo bạn có scene "MainMenuScene" trong dự án
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
