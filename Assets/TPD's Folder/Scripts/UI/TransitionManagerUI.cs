using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TransitionManagerUI : MonoBehaviour
{
    public CinemachineVirtualCamera currentCamera;

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
        Loader.Load(Loader.Scene.SingleplayerScene);
    }
    
    public void Multiplayer()
    {
        Loader.Load(Loader.Scene.ConnectToServer);
    }

    public void Exit()
    {
        Application.Quit();
    }

}
