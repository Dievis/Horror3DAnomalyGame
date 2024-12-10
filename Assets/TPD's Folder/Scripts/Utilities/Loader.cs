//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using Photon.Pun;

//public static class Loader
//{
//    public enum Scene
//    {
//        MainMenuScene,
//        SingleplayerScene,
//        //MultiplayerScene,
//        LoadingScene,
//        LobbyScene,
//        Game
//    }

//    private static Scene targetScene;

//    // Biến này dùng để xác định scene cần load tiếp theo
//    private static Scene nextScene;

//    // Chuyển cảnh cục bộ
//    public static void Load(Loader.Scene targetScene, Loader.Scene nextSceneAfterLoading)
//    {
//        Loader.targetScene = targetScene;
//        Loader.nextScene = nextSceneAfterLoading;

//        // Chuyển đến LoadingScene trước khi vào scene đích
//        SceneManager.LoadScene(Loader.Scene.LoadingScene.ToString());
//    }


//    //// Chuyển cảnh sử dụng Photon PUN 2
//    //public static void LoadNetwork(Scene targetScene)
//    //{
//    //    Loader.targetScene = targetScene;

//    //    // PhotonNetwork.LoadLevel sẽ sync cảnh cho tất cả người chơi trong cùng room
//    //    PhotonNetwork.LoadLevel(targetScene.ToString());
//    //}

//    // Callback sau khi LoadingScene hoàn tất
//    public static void LoaderCallback()
//    {
//        // Kiểm tra xem có kết nối Photon và đã vào phòng hay chưa
//        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
//        {
//            // Load level bằng Photon (chuyển đến Multiplayer game)
//            PhotonNetwork.LoadLevel(nextScene.ToString());
//        }
//        else
//        {
//            // Nếu không dùng Photon, load scene bình thường
//            SceneManager.LoadScene(nextScene.ToString());
//        }
//    }

//}

using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public static class Loader
{
    public enum Scene
    {
        MainMenuScene,
        SingleplayerScene,
        LoadingScene,
        LobbyScene,
        Game
    }

    private static Scene targetScene;
    private static Scene nextScene;

    // Hàm load scene và xác định scene tiếp theo
    public static void Load(Loader.Scene targetScene, Loader.Scene nextSceneAfterLoading)
    {
        Loader.targetScene = targetScene;
        Loader.nextScene = nextSceneAfterLoading;

        // Chuyển đến LoadingScene
        SceneManager.LoadScene(Loader.Scene.LoadingScene.ToString());
    }

    // Callback sau khi LoadingScene hoàn tất
    public static void LoaderCallback()
    {
        // Đảm bảo đã kết nối Photon và vào phòng
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            // Chỉ load scene tiếp theo nếu đã kết nối và vào phòng
            PhotonNetwork.LoadLevel(nextScene.ToString());
        }
        else if (SceneManager.GetSceneByName(targetScene.ToString()) != null)
        {
            // Nếu không kết nối Photon, sử dụng SceneManager để load scene
            SceneManager.LoadScene(nextScene.ToString());
        }
        else
        {
            Debug.LogError("Scene " + targetScene + " is not added to Build Settings.");
        }
    }
}
