using UnityEngine;
using Photon.Pun;
using Kino;
using System.Collections;

public class AnomalyInteraction : MonoBehaviourPunCallbacks
{
    [Header("Anomaly Interaction")] // Tương tác với anomaly
    public Camera playerCamera;  // Camera của người chơi
    public float interactionDistance = 10f;  // Khoảng cách tối đa để tương tác
    private GameObject currentAnomaly = null;  // Anomaly hiện tại mà người chơi đang nhìn vào
    private float lookTime = 0f;  // Thời gian nhìn vào anomaly
    private bool isLookingAtAnomaly = false;  // Kiểm tra xem có đang nhìn vào anomaly hay không

    [Header("Audio & Visual Effects")] // Hiệu ứng âm thanh và hình ảnh
    public AudioSource anomalySound;  // Âm thanh phát ra khi anomaly biến mất
    private AnalogGlitch analogGlitch;  // Tham chiếu đến script AnalogGlitch

    private CameraUI cameraUI;  // Tham chiếu đến CameraUI để kiểm tra trạng thái quay video

    // Tham chiếu đến AnomalyManager
    private AnomalyManager anomalyManager;

    void Start()
    {
        // Lấy tham chiếu đến AnalogGlitch trên camera
        analogGlitch = playerCamera.GetComponent<AnalogGlitch>();

        // Kiểm tra nếu AnalogGlitch không được gán vào camera
        if (analogGlitch == null)
        {
            Debug.LogError("AnalogGlitch component is missing on the camera.");
        }

        // Lấy tham chiếu đến CameraUI
        cameraUI = GetComponentInParent<CameraUI>();

        // Kiểm tra nếu CameraUI không được gán vào camera
        if (cameraUI == null)
        {
            Debug.LogError("CameraUI component is missing on the camera.");
        }

        // Lấy tham chiếu đến AnomalyManager
        anomalyManager = FindObjectOfType<AnomalyManager>();  // Tìm AnomalyManager trong scene

        // Kiểm tra nếu AnomalyManager không được gán
        if (anomalyManager == null)
        {
            Debug.LogError("AnomalyManager is missing in the scene.");
        }
    }

    void Update()
    {
        // Kiểm tra trạng thái quay video
        if (!cameraUI.isRecording)
        {
            // Nếu không đang quay, không làm gì khi nhìn vào anomaly
            return;
        }

        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        // Kiểm tra raycast có va chạm với đối tượng nào không
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            GameObject hitObject = hit.collider.gameObject;

            // Nếu raycast trúng đối tượng anomaly, lưu lại đối tượng đó
            if (hitObject.CompareTag("Anomaly"))
            {
                currentAnomaly = hitObject;

                // Tăng thời gian nhìn vào anomaly
                if (!isLookingAtAnomaly)
                {
                    isLookingAtAnomaly = true;
                    lookTime = 0f;  // Reset lại thời gian nhìn
                }

                lookTime += Time.deltaTime;

                // Nếu nhìn vào anomaly quá 3 giây
                if (lookTime >= 3f)
                {
                    // Gửi RPC để xóa anomaly trên tất cả client
                    photonView.RPC("RequestAnomalyDestruction", RpcTarget.All, currentAnomaly.GetPhotonView().ViewID);

                    // Phát âm thanh (nếu có)
                    if (anomalySound != null)
                    {
                        anomalySound.Play();
                    }

                    // Kích hoạt glitch effect trên tất cả client
                    photonView.RPC("ActivateGlitchEffect", RpcTarget.All);

                    // Gửi thông báo đến GameManager để tăng số lượng anomaly đã xóa
                    photonView.RPC("NotifyAnomalyFound", RpcTarget.All);

                    // Reset lại trạng thái nhìn vào anomaly
                    isLookingAtAnomaly = false;
                }
            }
            else
            {
                // Nếu không phải anomaly, reset trạng thái
                currentAnomaly = null;
                isLookingAtAnomaly = false;
                lookTime = 0f;
            }
        }
        else
        {
            // Nếu raycast không trúng đối tượng nào, reset trạng thái
            currentAnomaly = null;
            isLookingAtAnomaly = false;
            lookTime = 0f;
        }
    }

    [PunRPC]
    public void RequestAnomalyDestruction(int anomalyViewID)
    {
        // Chỉ Master Client mới xử lý yêu cầu xóa anomaly
        if (PhotonNetwork.IsMasterClient)
        {
            // Gọi phương thức DestroyAnomaly từ AnomalyManager
            anomalyManager.DestroyAnomaly(anomalyViewID);
        }
    }

    // RPC để kích hoạt glitch effect
    [PunRPC]
    private void ActivateGlitchEffect()
    {
        if (analogGlitch != null)
        {
            // Kích hoạt glitch effect
            analogGlitch.scanLineJitter = 1.0f;
            analogGlitch.verticalJump = 1.0f;
            analogGlitch.horizontalShake = 0.5f;
            analogGlitch.colorDrift = 0.7f;

            // Giữ glitch trong một khoảng thời gian ngắn
            StartCoroutine(ResetGlitchEffect());
        }
    }

    // Coroutine để reset glitch effect
    private IEnumerator ResetGlitchEffect()
    {
        // Giữ glitch effect trong khoảng thời gian
        yield return new WaitForSeconds(0.5f);

        // Tắt hiệu ứng glitch sau khi xóa anomaly
        if (analogGlitch != null)
        {
            analogGlitch.scanLineJitter = 0f;
            analogGlitch.verticalJump = 0f;
            analogGlitch.horizontalShake = 0f;
            analogGlitch.colorDrift = 0f;
        }
    }
}
