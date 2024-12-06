using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonFlashLight : MonoBehaviourPunCallbacks
{
    [SerializeField] private Light flashlight; // Kéo thả Spot Light vào đây từ Inspector
    private bool isFlashlightOn = false;

    void Start()
    {
        // Đảm bảo chỉ người sở hữu (local player) mới được phép điều khiển đèn pin
        if (!photonView.IsMine)
        {
            flashlight.enabled = false; // Tắt đèn với các người chơi khác
        }
    }

    void Update()
    {
        // Kiểm tra nếu là local player và nhận input để bật/tắt đèn pin
        if (photonView.IsMine && Input.GetKeyDown(KeyCode.F))
        {
            ToggleFlashlight();
        }
    }

    private void ToggleFlashlight()
    {
        isFlashlightOn = !isFlashlightOn; // Đổi trạng thái
        flashlight.enabled = isFlashlightOn;

        // Gửi trạng thái đèn pin tới các người chơi khác
        photonView.RPC("SyncFlashlight", RpcTarget.All, isFlashlightOn);
    }

    [PunRPC]
    private void SyncFlashlight(bool state)
    {
        isFlashlightOn = state;
        flashlight.enabled = state;
    }
}
