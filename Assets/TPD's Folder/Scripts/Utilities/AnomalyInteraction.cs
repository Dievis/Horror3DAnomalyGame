//using System.Collections;
//using UnityEngine;
//using Photon.Pun;
//using Kino;

//public class AnomalyInteraction : MonoBehaviourPunCallbacks
//{
//    public Camera playerCamera;  // Camera của người chơi
//    public float interactionDistance = 10f;  // Khoảng cách tối đa để tương tác
//    private GameObject currentAnomaly = null;  // Anomaly hiện tại mà người chơi đang nhìn vào
//    private float lookTime = 0f;  // Thời gian nhìn vào anomaly
//    private bool isLookingAtAnomaly = false;  // Kiểm tra xem có đang nhìn vào anomaly hay không

//    public AudioSource anomalySound;  // Âm thanh phát ra khi anomaly biến mất
//    private AnalogGlitch analogGlitch;  // Tham chiếu đến script AnalogGlitch

//    void Start()
//    {
//        // Lấy tham chiếu đến AnalogGlitch trên camera
//        analogGlitch = playerCamera.GetComponent<AnalogGlitch>();

//        // Kiểm tra nếu AnalogGlitch không được gán vào camera
//        if (analogGlitch == null)
//        {
//            Debug.LogError("AnalogGlitch component is missing on the camera.");
//        }
//    }

//    void Update()
//    {
//        RaycastHit hit;
//        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

//        // Kiểm tra raycast có va chạm với đối tượng nào không
//        if (Physics.Raycast(ray, out hit, interactionDistance))
//        {
//            GameObject hitObject = hit.collider.gameObject;

//            // Nếu raycast trúng đối tượng anomaly, lưu lại đối tượng đó
//            if (hitObject.CompareTag("Anomaly"))
//            {
//                currentAnomaly = hitObject;

//                // Tăng thời gian nhìn vào anomaly
//                if (!isLookingAtAnomaly)
//                {
//                    isLookingAtAnomaly = true;
//                    lookTime = 0f;  // Reset lại thời gian nhìn
//                }

//                lookTime += Time.deltaTime;

//                // Nếu nhìn vào anomaly quá 3 giây
//                if (lookTime >= 3f)
//                {
//                    // Phát âm thanh ghê rợn (Chỉ thực hiện ở client đầu tiên)
//                    if (anomalySound != null && photonView.IsMine)
//                    {
//                        Debug.Log("Chơi nhạc anomaly...");
//                        anomalySound.Play();
//                    }
//                    else
//                    {
//                        Debug.LogError("Âm thanh Anomaly ếu có.");
//                    }

//                    // Kích hoạt glitch effect (Gửi RPC để các client khác cũng nhận được hiệu ứng)
//                    photonView.RPC("ActivateGlitchEffect", RpcTarget.All);

//                    // Gọi lệnh RPC để xóa anomaly trên tất cả client
//                    photonView.RPC("DestroyAnomaly", RpcTarget.All, currentAnomaly.GetPhotonView().ViewID);

//                    // Reset lại trạng thái nhìn vào anomaly
//                    isLookingAtAnomaly = false;
//                }
//            }
//            else
//            {
//                // Nếu không phải anomaly, reset trạng thái
//                currentAnomaly = null;
//                isLookingAtAnomaly = false;
//                lookTime = 0f;
//            }
//        }
//        else
//        {
//            // Nếu raycast không trúng đối tượng nào, reset trạng thái
//            currentAnomaly = null;
//            isLookingAtAnomaly = false;
//            lookTime = 0f;
//        }
//    }

//    // RPC để xóa anomaly trên tất cả client
//    [PunRPC]
//    private void DestroyAnomaly(int anomalyViewID)
//    {
//        //Chỗ này dùng try catch vì thật ra nó lỗi nhưng nó vẫn chạy đúng như mong muốn nên dùng try catch để in ra debug log vài dòng thay vì một hàng đỏ lè
//        PhotonView anomalyPhotonView = PhotonView.Find(anomalyViewID);

//        if (anomalyPhotonView != null)
//        {
//            PhotonNetwork.Destroy(anomalyPhotonView.gameObject);

//        }
//    }



//    // RPC để kích hoạt glitch effect
//    [PunRPC]
//    private void ActivateGlitchEffect()
//    {
//        if (analogGlitch != null)
//        {
//            // Kích hoạt glitch effect
//            analogGlitch.scanLineJitter = 1.0f;
//            analogGlitch.verticalJump = 1.0f;
//            analogGlitch.horizontalShake = 0.5f;

//            // Tăng cường độ color drift đến tối đa khi nhìn vào anomaly
//            analogGlitch.colorDrift = 0.7f;

//            // Giữ glitch trong một khoảng thời gian ngắn
//            StartCoroutine(ResetGlitchEffect());
//        }
//    }

//    // Coroutine để reset glitch effect
//    private IEnumerator ResetGlitchEffect()
//    {
//        // Giữ glitch effect trong khoảng thời gian
//        yield return new WaitForSeconds(0.5f);

//        // Tắt hiệu ứng glitch sau khi xóa anomaly
//        if (analogGlitch != null)
//        {
//            analogGlitch.scanLineJitter = 0f;
//            analogGlitch.verticalJump = 0f;
//            analogGlitch.horizontalShake = 0f;

//            // Reset color drift về 0
//            analogGlitch.colorDrift = 0f;
//        }
//    }
//}

//using UnityEngine;
//using Photon.Pun;
//using Kino;
//using System.Collections;

//public class AnomalyInteraction : MonoBehaviourPunCallbacks
//{
//    public Camera playerCamera;  // Camera của người chơi
//    public float interactionDistance = 10f;  // Khoảng cách tối đa để tương tác
//    private GameObject currentAnomaly = null;  // Anomaly hiện tại mà người chơi đang nhìn vào
//    private float lookTime = 0f;  // Thời gian nhìn vào anomaly
//    private bool isLookingAtAnomaly = false;  // Kiểm tra xem có đang nhìn vào anomaly hay không

//    public AudioSource anomalySound;  // Âm thanh phát ra khi anomaly biến mất
//    private AnalogGlitch analogGlitch;  // Tham chiếu đến script AnalogGlitch
//    private AnomalyController anomalyController;  // Tham chiếu đến AnomalyController

//    void Start()
//    {
//        // Lấy tham chiếu đến AnalogGlitch trên camera
//        analogGlitch = playerCamera.GetComponent<AnalogGlitch>();

//        // Kiểm tra nếu AnalogGlitch không được gán vào camera
//        if (analogGlitch == null)
//        {
//            Debug.LogError("AnalogGlitch component is missing on the camera.");
//        }

//        // Lấy tham chiếu đến AnomalyController trên anomaly
//        anomalyController = GetComponent<AnomalyController>();
//        if (anomalyController == null)
//        {
//            Debug.LogError("AnomalyController component is missing on the anomaly object.");
//        }
//    }

//    void Update()
//    {
//        RaycastHit hit;
//        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

//        // Kiểm tra raycast có va chạm với đối tượng nào không
//        if (Physics.Raycast(ray, out hit, interactionDistance))
//        {
//            GameObject hitObject = hit.collider.gameObject;

//            // Nếu raycast trúng đối tượng anomaly, lưu lại đối tượng đó
//            if (hitObject.CompareTag("Anomaly"))
//            {
//                currentAnomaly = hitObject;

//                // Tăng thời gian nhìn vào anomaly
//                if (!isLookingAtAnomaly)
//                {
//                    isLookingAtAnomaly = true;
//                    lookTime = 0f;  // Reset lại thời gian nhìn
//                }

//                lookTime += Time.deltaTime;

//                // Nếu nhìn vào anomaly quá 3 giây
//                if (lookTime >= 3f)
//                {
//                    // Gửi RPC để thay đổi trạng thái anomaly cho tất cả client
//                    photonView.RPC("SwitchAnomalyState", RpcTarget.All, true); // true để chuyển về trạng thái normal

//                    // Reset lại trạng thái nhìn vào anomaly
//                    isLookingAtAnomaly = false;
//                }
//            }
//            else
//            {
//                // Nếu không phải anomaly, reset trạng thái
//                currentAnomaly = null;
//                isLookingAtAnomaly = false;
//                lookTime = 0f;
//            }
//        }
//        else
//        {
//            // Nếu raycast không trúng đối tượng nào, reset trạng thái
//            currentAnomaly = null;
//            isLookingAtAnomaly = false;
//            lookTime = 0f;
//        }
//    }

//    // RPC để kích hoạt glitch effect
//    [PunRPC]
//    private void ActivateGlitchEffect()
//    {
//        if (analogGlitch != null)
//        {
//            // Kích hoạt glitch effect
//            analogGlitch.scanLineJitter = 1.0f;
//            analogGlitch.verticalJump = 1.0f;
//            analogGlitch.horizontalShake = 0.5f;

//            // Tăng cường độ color drift đến tối đa khi nhìn vào anomaly
//            analogGlitch.colorDrift = 0.7f;

//            // Giữ glitch trong một khoảng thời gian ngắn
//            StartCoroutine(ResetGlitchEffect());
//        }
//    }

//    // Coroutine để reset glitch effect
//    private IEnumerator ResetGlitchEffect()
//    {
//        // Giữ glitch effect trong khoảng thời gian
//        yield return new WaitForSeconds(0.5f);

//        // Tắt hiệu ứng glitch sau khi xóa anomaly
//        if (analogGlitch != null)
//        {
//            analogGlitch.scanLineJitter = 0f;
//            analogGlitch.verticalJump = 0f;
//            analogGlitch.horizontalShake = 0f;

//            // Reset color drift về 0
//            analogGlitch.colorDrift = 0f;
//        }
//    }
//}

using UnityEngine;
using Photon.Pun;
using Kino;
using System.Collections;

public class AnomalyInteraction : MonoBehaviourPunCallbacks
{
    public Camera playerCamera;  // Camera của người chơi
    public float interactionDistance = 10f;  // Khoảng cách tối đa để tương tác
    private GameObject currentAnomaly = null;  // Anomaly hiện tại mà người chơi đang nhìn vào
    private float lookTime = 0f;  // Thời gian nhìn vào anomaly
    private bool isLookingAtAnomaly = false;  // Kiểm tra xem có đang nhìn vào anomaly hay không

    public AudioSource anomalySound;  // Âm thanh phát ra khi anomaly biến mất
    private AnalogGlitch analogGlitch;  // Tham chiếu đến script AnalogGlitch

    void Start()
    {
        // Lấy tham chiếu đến AnalogGlitch trên camera
        analogGlitch = playerCamera.GetComponent<AnalogGlitch>();

        // Kiểm tra nếu AnalogGlitch không được gán vào camera
        if (analogGlitch == null)
        {
            Debug.LogError("AnalogGlitch component is missing on the camera.");
        }
    }

    void Update()
    {
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
                    photonView.RPC("DestroyAnomaly", RpcTarget.All, currentAnomaly.GetPhotonView().ViewID);

                    // Phát âm thanh (nếu có)
                    if (anomalySound != null)
                    {
                        anomalySound.Play();
                    }

                    // Kích hoạt glitch effect trên tất cả client
                    photonView.RPC("ActivateGlitchEffect", RpcTarget.All);

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

    // RPC để xóa anomaly trên tất cả client
    [PunRPC]
    private void DestroyAnomaly(int anomalyViewID)
    {
        PhotonView anomalyPhotonView = PhotonView.Find(anomalyViewID);
        if (anomalyPhotonView != null)
        {
            PhotonNetwork.Destroy(anomalyPhotonView.gameObject);
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
