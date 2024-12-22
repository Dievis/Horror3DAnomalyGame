using Photon.Pun;
using UnityEngine;

public class AnomalyManager : MonoBehaviourPunCallbacks
{
    // Phương thức để xóa anomaly
    //[PunRPC]
    //public void DestroyAnomaly(int anomalyViewID)
    //{
    //    // Chỉ Master Client mới xử lý xóa anomaly
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        PhotonView anomalyPhotonView = PhotonView.Find(anomalyViewID);
    //        if (anomalyPhotonView != null)
    //        {
    //            PhotonNetwork.Destroy(anomalyPhotonView.gameObject);
    //        }
    //    }
    //}

    public void DestroyAnomaly(int anomalyViewID)
    {
        GameObject anomaly = PhotonView.Find(anomalyViewID)?.gameObject;
        if (anomaly != null)
        {
            // Xóa anomaly khỏi scene
            Destroy(anomaly);
            Debug.Log("Anomaly destroyed: " + anomaly.name);
        }
        else
        {
            Debug.Log("Anomaly with ID " + anomalyViewID + " not found.");
        }
    }

}
