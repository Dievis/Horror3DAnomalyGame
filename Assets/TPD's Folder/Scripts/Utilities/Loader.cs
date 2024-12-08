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
        LobbyScene,
        ConnectToServer
    }

    private static Scene targetScene;

    // Chuyển cảnh cục bộ
    public static void Load(Loader.Scene targetScene)
    {
        Loader.targetScene = targetScene;

        // Chuyển đến LoadingScene trước khi vào scene đích
        SceneManager.LoadScene(Loader.Scene.LoadingScene.ToString());
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
            // Sau khi vào LoadingScene, load scene Multiplayer thông qua Photon
            PhotonNetwork.LoadLevel(targetScene.ToString());
        }
        else
        {
            // Khi không kết nối Photon, dùng SceneManager để load scene offline
            SceneManager.LoadScene(targetScene.ToString());
        }
    }

}
