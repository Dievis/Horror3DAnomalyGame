using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;

    public void Start()
    {
        ActiveMainMenu(true);
    }

    public void ActiveMainMenu(bool state)
    {
        mainMenu.SetActive(state);
        optionsMenu.SetActive(!state);
    }
    public void Play()
    {
        SceneManager.LoadScene(1);
    }
    public void Quit()
    {
        Debug.Log("EXIT!");
        Application.Quit();
    }

}
