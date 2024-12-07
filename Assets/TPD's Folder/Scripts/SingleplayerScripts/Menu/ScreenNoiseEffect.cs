using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenNoiseEffect : MonoBehaviour
{
    [SerializeField] GameObject endMenuUI;//chỗ này bỏ menu End khi thắng vào
    public RawImage noiseImage; // Hình ảnh nhiễu màn hình
    public RawImage whiteImage; // Hình ảnh nhiễu màn hình
    private float noiseSpeed = 5f; // Tốc độ di chuyển của nhiễu màn hình
    private float fadeSpeed = 3f; // Tốc độ mờ dần của màn hình
    public AudioSource backgroundmusic;
    private float noiseOffset = 0f;
    private float fadeAlpha = 0f;
    private bool isFading = false;
    private bool isShowingEndScreen = false; // Biến kiểm tra xem đã hiển thị màn hình kết thúc chưa
    private bool isEndMenuActive = false; // Thêm biến kiểm tra end menu có hiện lên không
    public AudioSource noiseSound;
    public AudioSource whiteScreenSound;
    public AudioClip clip1;
    private bool hasPlayedSound = false; // Thêm biến kiểm tra này
    void Update()
    {
        if (isFading)
        {
            // Tăng giá trị fadeAlpha dần lên tới 1
            fadeAlpha += fadeSpeed * Time.deltaTime;
            fadeAlpha = Mathf.Clamp01(fadeAlpha);

            // Thay đổi màu sắc của hình ảnh nhiễu màn hình
            noiseImage.color = new Color(1f, 1f, 1f, fadeAlpha);
        }
        else if (isShowingEndScreen)
        {
            // Hiển thị màn hình trắng
            noiseImage.color = Color.Lerp(noiseImage.color, Color.white, fadeSpeed * Time.deltaTime);
        }
        else
        {
            // Di chuyển nhiễu màn hình theo tốc độ noiseSpeed
            noiseOffset += noiseSpeed * Time.deltaTime;
            noiseImage.material.SetTextureOffset("_MainTex", new Vector2(noiseOffset, noiseOffset));
        }

    }
    void Start()
    {

        // Tạo một bản sao của Material
        Material newMaterial = new Material(noiseImage.material);

        // Gán bản sao cho noiseImage
        noiseImage.material = newMaterial;
    }
    public void StartFadeEffect()
    {


        noiseSound.Play(); // Chơi âm thanh khi bắt đầu hiệu ứng
        isFading = true;
        fadeAlpha = 0f;
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        float fadeDuration = 0.5f;
        float targetAlpha = 0.5f;
        float maxAlpha = 1f;
        bool showWhiteScreen = false;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            float fadeAlpha = Mathf.Lerp(targetAlpha, maxAlpha, t);
            noiseImage.color = new Color(1f, 1f, 1f, fadeAlpha);

            if (fadeAlpha >= maxAlpha && !showWhiteScreen)
            {
                // Hiển thị màn hình trắng khi đạt đến độ mờ tối đa
                StartCoroutine(ShowWhiteScreen());
                showWhiteScreen = true;
            }

            yield return null;
        }

        noiseImage.gameObject.SetActive(true); // Hiển thị hình ảnh nhiễu màn hình
    }

    private IEnumerator ShowWhiteScreen()
    {

        float elapsedTime = 0f;
        float whiteScreenDuration = 2f;
        Color initialColor = noiseImage.color;
        Color targetColor = Color.white;

        while (elapsedTime < whiteScreenDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / whiteScreenDuration;
            noiseImage.color = Color.Lerp(initialColor, targetColor, t);

            yield return null;
        }

        // Hiển thị menu end game tại đây
    }

    public void StartNoiseEffect()
    {
        noiseSound.Stop(); // Dừng âm thanh khi kết thúc hiệu ứng
        isFading = false;
        isShowingEndScreen = false;
        noiseOffset = 0f;
        fadeAlpha = 0f;
        noiseImage.gameObject.SetActive(false);

    }
    /*
    public void ShowWhiteImage()
    {
        whiteImage.gameObject.SetActive(true);
        Time.timeScale = 0;
    }
    */
    public void ShowEndGameMenu()
    {
        StartCoroutine(ShowWhiteAndEndMenu());
        Cursor.visible = true; // Hiển thị con trỏ chuột
        Cursor.lockState = CursorLockMode.None; // Thêm dòng này

    }

    private IEnumerator ShowWhiteAndEndMenu()
    {
        whiteImage.gameObject.SetActive(true);
        if (!hasPlayedSound) // Kiểm tra nếu âm thanh chưa được phát
        {

            whiteScreenSound.PlayOneShot(clip1); // Chơi âm thanh khi màn hình trắng xuất hiện
            hasPlayedSound = true; // Đánh dấu rằng âm thanh đã được phát
        }
        yield return new WaitForSeconds(2f);

        whiteImage.gameObject.SetActive(false);
        isEndMenuActive = true;
        endMenuUI.gameObject.SetActive(true);
        Time.timeScale = 0;
        backgroundmusic.Pause();


    }
}
