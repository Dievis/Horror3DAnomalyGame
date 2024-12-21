using UnityEngine;
using TMPro;

public class LoadingTextEffect : MonoBehaviour
{
    public TextMeshProUGUI loadingText;
    public float speed = 0.5f;

    private float timer;
    private int dotCount;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= speed)
        {
            dotCount = (dotCount + 1) % 4; // 0, 1, 2, 3
            loadingText.text = "Đang tải" + new string('.', dotCount);
            timer = 0;
        }
    }
}
