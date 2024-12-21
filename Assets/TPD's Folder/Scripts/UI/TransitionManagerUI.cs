using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManagerUI : MonoBehaviour
{
    public CinemachineVirtualCamera currentCamera;

    private void Awake()
    {
        UnlockCursor();
    }

    private void Start()
    {
        // Chỉ tăng priority của camera nếu camera không phải là camera hiện tại
        if (currentCamera != null)
            currentCamera.Priority++;
    }

    public void UpdateCamera(CinemachineVirtualCamera target)
    {
        // Kiểm tra nếu camera mới khác camera hiện tại để tránh thay đổi không cần thiết
        if (currentCamera != target)
        {
            // Giảm priority của camera cũ
            if (currentCamera != null)
                currentCamera.Priority--;

            // Cập nhật camera hiện tại và tăng priority
            currentCamera = target;
            currentCamera.Priority++;
        }
    }

    public void Singleplayer()
    {
        // Gọi phương thức LoadScene chung để tránh mã trùng lặp
        LoadScene(Loader.Scene.LoadingScene, Loader.Scene.SingleplayerScene);
    }

    public void Multiplayer()
    {
        // Tương tự như Singleplayer nhưng cho Multiplayer
        LoadScene(Loader.Scene.LoadingScene, Loader.Scene.LobbyScene);
    }

    public void Exit()
    {
        Application.Quit();
    }

    private void UnlockCursor()
    {
        // Mở khóa con trỏ khi cần thiết
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void LoadScene(Loader.Scene loadingScene, Loader.Scene targetScene)
    {
        // Hiển thị màn hình loading và chuyển đến scene mục tiêu
        Loader.Load(loadingScene, targetScene);
    }
}
