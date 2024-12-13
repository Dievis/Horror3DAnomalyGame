using UnityEngine;
using Photon.Pun;

public class AnomalyController : MonoBehaviourPunCallbacks
{
    public Transform normal;  // Transform của object normal
    public Transform anomaly; // Transform của object anomaly

    private bool isInAnomalyState = true; // Kiểm tra trạng thái của anomaly (true: anomaly, false: normal)

    void Start()
    {
        // Đảm bảo cả hai đối tượng normal và anomaly được thiết lập
        if (normal == null || anomaly == null)
        {
            Debug.LogError("Normal hoặc Anomaly không được gán trong Inspector.");
        }

        // Đảm bảo bắt đầu với anomaly được enable
        if (anomaly != null && normal != null)
        {
            anomaly.gameObject.SetActive(true); // Bật anomaly
            normal.gameObject.SetActive(false); // Tắt normal
        }
    }

    // Gọi khi cần chuyển trạng thái của anomaly
    public void ChangeAnomalyState(bool toNormal)
    {
        if (photonView.IsMine)
        {
            if (toNormal)
            {
                // Disable anomaly và enable normal
                anomaly.gameObject.SetActive(false);
                normal.gameObject.SetActive(true);
            }
            else
            {
                // Enable anomaly và disable normal
                anomaly.gameObject.SetActive(true);
                normal.gameObject.SetActive(false);
            }
        }
    }

    // RPC để thay đổi trạng thái anomaly cho tất cả client
    [PunRPC]
    public void SwitchAnomalyState(bool toNormal)
    {
        ChangeAnomalyState(toNormal);
    }
}
