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
