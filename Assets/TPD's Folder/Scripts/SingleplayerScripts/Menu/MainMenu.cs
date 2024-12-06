using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //start game
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
    //Exit the game
    public void ExitGame()
    {
        Debug.Log("EXIT!");
        Application.Quit(); 
    }
    public void Continue()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
