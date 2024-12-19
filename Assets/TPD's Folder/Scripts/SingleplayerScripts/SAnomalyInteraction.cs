using UnityEngine;
using Kino;
using System.Collections;

public class SAnomalyInteraction : MonoBehaviour
{
    [Header("Anomaly Interaction")] // Tương tác với anomaly
    public Camera playerCamera;  // Camera của người chơi
    public float interactionDistance = 10f;  // Khoảng cách tối đa để tương tác với anomaly
    private GameObject currentAnomaly = null;  // Anomaly hiện tại mà người chơi đang nhìn vào
    private float lookTime = 0f;  // Thời gian người chơi nhìn vào anomaly
    private bool isLookingAtAnomaly = false;  // Kiểm tra xem người chơi có đang nhìn vào anomaly hay không

    [Header("Audio & Visual Effects")] // Hiệu ứng âm thanh và hình ảnh
    public AudioSource anomalySound;  // Âm thanh phát ra khi anomaly bị phá hủy
    private AnalogGlitch analogGlitch;  // Tham chiếu đến script AnalogGlitch (hiệu ứng glitch)

    private SCameraUI cameraUI;  // Tham chiếu đến CameraUI để kiểm tra trạng thái quay video

    void Start()
    {
        // Lấy tham chiếu đến AnalogGlitch trên camera
        analogGlitch = playerCamera.GetComponent<AnalogGlitch>();

        if (analogGlitch == null)
        {
            Debug.LogError("AnalogGlitch component is missing on the camera.");  // Nếu không tìm thấy, in lỗi
        }

        // Lấy tham chiếu đến CameraUI
        cameraUI = GetComponentInParent<SCameraUI>();

        if (cameraUI == null)
        {
            Debug.LogError("CameraUI component is missing on the camera.");  // Nếu không tìm thấy, in lỗi
        }
    }

    void Update()
    {
        // Kiểm tra xem người chơi có đang quay video không
        if (!cameraUI.isRecording)
        {
            return;  // Nếu không quay video, không làm gì cả
        }

        // Tạo một raycast từ vị trí chuột của người chơi trên màn hình
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        // Kiểm tra xem ray có va chạm với đối tượng trong phạm vi tương tác không
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            GameObject hitObject = hit.collider.gameObject;  // Lấy đối tượng mà ray va chạm

            // Kiểm tra nếu đối tượng là anomaly
            if (hitObject.CompareTag("Anomaly"))
            {
                currentAnomaly = hitObject;  // Gán anomaly hiện tại

                if (!isLookingAtAnomaly)
                {
                    isLookingAtAnomaly = true;  // Đánh dấu là đang nhìn vào anomaly
                    lookTime = 0f;  // Reset thời gian nhìn
                }

                lookTime += Time.deltaTime;  // Tăng thời gian nhìn

                // Nếu nhìn vào anomaly đủ lâu (3 giây), xóa anomaly và kích hoạt hiệu ứng
                if (lookTime >= 3f)
                {
                    DestroyAnomaly(currentAnomaly);  // Xóa anomaly

                    if (anomalySound != null)
                    {
                        anomalySound.Play();  // Phát âm thanh khi anomaly bị xóa
                    }

                    ActivateGlitchEffect();  // Kích hoạt hiệu ứng glitch

                    SGameManager.instance.NotifyAnomalyFound(currentAnomaly);  // Thông báo cho GameManager rằng anomaly đã bị xóa

                    isLookingAtAnomaly = false;  // Đánh dấu không còn nhìn vào anomaly nữa
                }
            }
            else
            {
                ResetInteractionState();  // Nếu không phải anomaly, reset trạng thái tương tác
            }
        }
        else
        {
            ResetInteractionState();  // Nếu không có va chạm, reset trạng thái tương tác
        }
    }

    // Phương thức reset trạng thái tương tác khi không nhìn vào anomaly nữa
    private void ResetInteractionState()
    {
        currentAnomaly = null;  // Đặt lại anomaly hiện tại
        isLookingAtAnomaly = false;  // Đánh dấu không nhìn vào anomaly
        lookTime = 0f;  // Reset thời gian nhìn
    }

    // Phương thức xóa anomaly
    private void DestroyAnomaly(GameObject anomaly)
    {
        if (anomaly != null)
        {
            Destroy(anomaly);  // Xóa anomaly
            Debug.Log("Anomaly destroyed: " + anomaly.name);  // In thông báo đã xóa anomaly
        }
        else
        {
            Debug.LogWarning("Attempted to destroy a null anomaly.");  // Cảnh báo nếu cố gắng xóa anomaly null
        }
    }

    // Phương thức kích hoạt hiệu ứng glitch
    private void ActivateGlitchEffect()
    {
        if (analogGlitch != null)
        {
            // Cài đặt các giá trị hiệu ứng glitch
            analogGlitch.scanLineJitter = 1.0f;
            analogGlitch.verticalJump = 1.0f;
            analogGlitch.horizontalShake = 0.5f;
            analogGlitch.colorDrift = 0.7f;

            // Reset hiệu ứng glitch sau một khoảng thời gian ngắn
            StartCoroutine(ResetGlitchEffect());
        }
    }

    // Coroutine để reset hiệu ứng glitch sau 0.5 giây
    private IEnumerator ResetGlitchEffect()
    {
        yield return new WaitForSeconds(0.5f);  // Đợi 0.5 giây

        if (analogGlitch != null)
        {
            // Đặt lại giá trị hiệu ứng glitch về mặc định
            analogGlitch.scanLineJitter = 0f;
            analogGlitch.verticalJump = 0f;
            analogGlitch.horizontalShake = 0f;
            analogGlitch.colorDrift = 0f;
        }
    }
}
