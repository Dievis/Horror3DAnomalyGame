using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuEG : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject pauseMenuUI2;
    public bool isGameOver = false;
    public bool hasWon = false; 

    void Start()
    {
        // Disable the pause menu and loading menus at the start of the game
        pauseMenuUI.SetActive(false);
        pauseMenuUI2.SetActive(false); // Make sure pauseMenuUI2 is initially disabled
    }

    void Update()
    {
        // If the game is over, show the appropriate pause menu
        if (isGameOver)
        {
            pauseMenuUI.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0; // Pause the game
        }
        else if (hasWon) // If the player has won
        {
            pauseMenuUI2.SetActive(true); // Show the win menu
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0; // Pause the game
        }
    }

    public void GameOver() // Call this function when the game is over
    {
        isGameOver = true;
    }

    public void WinGame() // Call this function when the player wins
    {
        hasWon = true;
    }

    public void Restart()
    {
        SceneManager.LoadScene("SingleplayerScene");
    }

    public void ExitToMenu()
    {
        Loader.Load(Loader.Scene.MainMenuScene);

    }
}
