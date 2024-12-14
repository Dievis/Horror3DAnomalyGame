using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManagerUI : MonoBehaviour
{
    public CinemachineVirtualCamera currentCamera;

    public void Awake()
    {
        UnlockCursor();
    }

    public void Start()
    {
        currentCamera.Priority++;
    }


    public void UpdateCamera(CinemachineVirtualCamera target)
    {
        currentCamera.Priority--;

        currentCamera = target;

        currentCamera.Priority++;
    }

    public void Singleplayer()
    {
        // Hiển thị panel loading khi bắt đầu chơi
        Loader.Load(Loader.Scene.LoadingScene, Loader.Scene.SingleplayerScene);
    }

    public void Multiplayer()
    {
        // Chuyển đến LoadingScene trước khi vào LobbyScene
        Loader.Load(Loader.Scene.LoadingScene, Loader.Scene.LobbyScene);

        //Chuyển thẳng đến lobbyScene
        //SceneManager.LoadScene("LobbyScene");
    }

    public void Exit()
    {
        Application.Quit();
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
