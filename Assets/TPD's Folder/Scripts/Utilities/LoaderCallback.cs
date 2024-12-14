using UnityEngine;

public class LoaderCallback : MonoBehaviour
{
    private bool isFirstUpdate = true;

    private void Start()
    {
        // Gọi LoaderCallback chỉ khi vào LoadingScene lần đầu tiên
        if (isFirstUpdate)
        {
            isFirstUpdate = false;
            Loader.LoaderCallback();  // Đảm bảo load scene tiếp theo
        }
    }

    private void Update()
    {
        // Chỉ gọi một lần khi LoadingScene đã hoàn tất
        if (isFirstUpdate)
        {
            isFirstUpdate = false;
            Loader.LoaderCallback();  // Đảm bảo load scene tiếp theo
        }
    }
}

//using UnityEngine;
//using Photon.Pun;
//using Photon.Realtime;

//public class LoaderCallback : MonoBehaviour
//{
//    private bool isFirstUpdate = true;

//    private void Start()
//    {
//        // Gọi LoaderCallback chỉ khi vào LoadingScene lần đầu tiên
//        if (isFirstUpdate)
//        {
//            isFirstUpdate = false;
//            CheckPhotonConnection();  // Kiểm tra kết nối và vào phòng
//        }
//    }

//    private void Update()
//    {
//        // Chỉ gọi một lần khi LoadingScene đã hoàn tất
//        if (isFirstUpdate)
//        {
//            isFirstUpdate = false;
//            CheckPhotonConnection();  // Kiểm tra kết nối và vào phòng
//        }
//    }

//    private void CheckPhotonConnection()
//    {
//        if (PhotonNetwork.IsConnected)
//        {
//            if (PhotonNetwork.InRoom)
//            {
//                // Nếu đã kết nối và vào phòng, gọi LoaderCallback để load scene tiếp theo
//                Loader.LoaderCallback();
//            }
//            else
//            {
//                Debug.LogError("Chưa vào phòng Photon.");
//                // Bạn có thể tự động tạo phòng hoặc yêu cầu người chơi vào phòng
//                PhotonNetwork.JoinOrCreateRoom("defaultRoom", new Photon.Realtime.RoomOptions(), TypedLobby.Default);
//            }
//        }
//        else
//        {
//            Debug.LogError("Chưa kết nối với Photon.");
//            // Tự động kết nối lại với Photon
//            PhotonNetwork.ConnectUsingSettings();
//        }
//    }
//}

