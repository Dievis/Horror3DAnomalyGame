using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public static class Loader
{
    public enum Scene
    {
        MainMenuScene,
        SingleplayerScene,
        MultiplayerScene,
        LoadingScene,
        Lobby,
        ConnectToServer
    }

    private static Scene targetScene;

    // Chuyển cảnh cục bộ
    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;

        // Nếu là Singleplayer hoặc Offline Scene
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    // Chuyển cảnh sử dụng Photon PUN 2
    public static void LoadNetwork(Scene targetScene)
    {
        Loader.targetScene = targetScene;

        // PhotonNetwork.LoadLevel sẽ sync cảnh cho tất cả người chơi trong cùng room
        PhotonNetwork.LoadLevel(targetScene.ToString());
    }

    // Callback sau khi LoadingScene hoàn tất
    public static void LoaderCallback()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            // Dùng PhotonNetwork.LoadLevel khi đang kết nối mạng
            PhotonNetwork.LoadLevel(targetScene.ToString());
        }
        else
        {
            // Dùng SceneManager khi offline hoặc không kết nối Photon
            SceneManager.LoadScene(targetScene.ToString());
        }
    }
}
