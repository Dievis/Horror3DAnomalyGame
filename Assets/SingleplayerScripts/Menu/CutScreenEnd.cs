using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutScreenEnd : MonoBehaviour
{
    [SerializeField] GameObject LoseMenuUI;//chỗ này bỏ menu End khi thua vào
    public RawImage loseImage;
    public VideoPlayer video;
    public AudioSource backgroundmusic;
    // public TriggerLose tgw;
    private bool isLoseMenuActive = false; // Biến mới để kiểm tra xem LoseMenu có đang được hiển thị hay không
    // Start is called before the first frame update
    void Start()
    {
        loseImage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (isLoseMenuActive && Input.GetKeyDown(KeyCode.Escape))
        {
            return;
        }
    }

    public void ShowCutScreen()
    {
        StartCoroutine(CutConCard());

    }

    public IEnumerator CutConCard()
    {
        loseImage.gameObject.SetActive(true);
        isLoseMenuActive = true; // Cập nhật trạng thái của LoseMenu

        backgroundmusic.Pause();

        yield return new WaitForSeconds(6f);


        loseImage.gameObject.SetActive(false);
        video.gameObject.SetActive(false);
        LoseMenuUI.gameObject.SetActive(true);

        Time.timeScale = 0;
        Cursor.visible = true; // Hiển thị con trỏ chuột
        Cursor.lockState = CursorLockMode.None; // Thêm dòng này


    }
}