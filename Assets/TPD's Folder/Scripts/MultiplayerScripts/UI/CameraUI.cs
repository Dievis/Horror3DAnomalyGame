using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun.Demo.PunBasics;

public class CameraUI : MonoBehaviourPunCallbacks
{
    public TMP_Text recText;          // Tham chiếu đến Text hiển thị chữ "REC"
    public Image circleImage;         // Tham chiếu đến Image hình tròn
    public float blinkSpeed = 0.5f;   // Tốc độ nhấp nháy (thay đổi nếu cần)

    public GameObject cameraUIPanel;  // Tham chiếu đến Panel chứa các UI cần hiển thị/ẩn
    public GameObject light;

    public Image BatteryImg;          // Tham chiếu đến Image hiển thị pin
    public Sprite FullBattery;        // Sprite cho pin đầy
    public Sprite HalfBattery;        // Sprite cho pin một nửa
    public Sprite ChargeBattery;      // Sprite cho pin cần sạc

    [Header("Battery level: 3 = Full, 2 = Half, 1 = Charge, 0 = Game over")]
    public int batteryLevel = 3;     // Mức pin: 3 là đầy, 2 là một nửa, 1 là cần sạc, 0 là thua
    private Coroutine blinkCoroutine;

    [Header("Time passed >= 30 : -1 battery level")]
    public float timePassed = 0f;    // Biến đếm thời gian trôi qua mỗi frame

    public bool isRecording = false;  // Trạng thái quay video

    private GameManager gameManager;

    void Start()
    {
        if (cameraUIPanel != null)
        {
            cameraUIPanel.SetActive(false);
        }

        isRecording = false;
        UpdateRecordingUI();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            UpdateBatteryLevel();

            if (Input.GetKeyDown(KeyCode.F))
            {
                TogglePanel();
            }
        }
    }

    public void UpdateRecordingUI()
    {
        if (isRecording)
        {
            recText.text = "REC";
            circleImage.color = Color.red;
            StartBlinking();
        }
        else
        {
            recText.text = "";
            circleImage.color = Color.clear;
            StopBlinking();
        }
    }

    public void StartRecording()
    {
        if (!photonView.IsMine) return;
        light.SetActive(true);
        isRecording = true;
        photonView.RPC("RPC_UpdateRecordingState", RpcTarget.All, isRecording);
    }

    public void StopRecording()
    {
        if (!photonView.IsMine) return;
        light.SetActive(false);
        isRecording = false;
        photonView.RPC("RPC_UpdateRecordingState", RpcTarget.All, isRecording);
    }

    [PunRPC]
    private void RPC_UpdateRecordingState(bool recording)
    {
        isRecording = recording;
        UpdateRecordingUI();
    }

    private void UpdateBatteryLevel()
    {
        if (isRecording)
        {
            timePassed += Time.deltaTime;

            if (timePassed >= 30f)
            {
                timePassed = 0f;
                photonView.RPC("RPC_DecreaseBatteryLevel", RpcTarget.All);
            }
        }

        UpdateBatteryUI();
    }

    [PunRPC]
    private void RPC_DecreaseBatteryLevel()
    {
        if (batteryLevel > 0)
        {
            batteryLevel--;
            Debug.Log("Battery Level: " + batteryLevel);
        }

        if (batteryLevel == 0)
        {
            StopRecording();
        }
    }

    public void UpdateBatteryUI()
    {
        if (batteryLevel == 3)
        {
            BatteryImg.sprite = FullBattery;
        }
        else if (batteryLevel == 2)
        {
            BatteryImg.sprite = HalfBattery;
        }
        else if (batteryLevel == 1)
        {
            BatteryImg.sprite = ChargeBattery;
        }
        else
        {
            photonView.RPC("RPC_BatteryDepleted", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_BatteryDepleted()
    {
        // Call GameManager to check if all players are out of battery
        gameManager.CheckAllPlayersOutOfBattery();
    }

    public void TogglePanel()
    {
        if (cameraUIPanel != null)
        {
            bool isActive = cameraUIPanel.activeSelf;
            cameraUIPanel.SetActive(!isActive);


            if (cameraUIPanel.activeSelf)
            {
                StartRecording();
            }
            else
            {
                StopRecording();
            }
        }
    }

    private IEnumerator BlinkCircle()
    {
        while (isRecording)
        {
            circleImage.color = Color.red;
            yield return new WaitForSeconds(blinkSpeed);
            circleImage.color = new Color(1f, 0f, 0f, 0f);
            yield return new WaitForSeconds(blinkSpeed);
        }
    }

    private void StartBlinking()
    {
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        blinkCoroutine = StartCoroutine(BlinkCircle());
    }

    private void StopBlinking()
    {
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        circleImage.color = Color.clear;
    }
}
