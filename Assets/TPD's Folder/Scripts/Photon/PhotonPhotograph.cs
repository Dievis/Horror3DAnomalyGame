using System.Collections;
using System.Collections.Generic;
using Photon.Pun; // Import Photon PUN
using UnityEngine;

public class PhotonPhotograph : MonoBehaviourPunCallbacks
{
    public GameObject photograph;
    public GameObject Plight;
    public AudioSource audioSource;
    public AudioClip sound1;
    private bool PlayerInZone;
    private bool isActive = false;
    private GameObject Hand;
    public bool onhand;
    public GameObject photoReach;
    [SerializeField] public PhotoCD _photoCD;

    void Start()
    {
        photograph.SetActive(true);
        Plight.SetActive(false);
        PlayerInZone = false;
        onhand = false;

        // Đảm bảo đây là đối tượng của người chơi hiện tại
        if (photonView.IsMine)
        {
            // Tìm GameObject Player có tag "Player"
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("Player GameObject with tag 'Player' not found!");
                return;
            }

            // Tìm CameraReach trong Player
            Transform cameraReach = player.transform.FindRecursive("CameraReach");
            if (cameraReach == null)
            {
                Debug.LogError("CameraReach không được tìm thấy! Kiểm tra cấu trúc Player.");
                return;
            }
            else
                photoReach = cameraReach.gameObject;

            //// Tìm RightHand trong Player
            //Transform rightHandTransform = player.transform.FindRecursive("RightHand");
            //if (rightHandTransform != null)
            //{
            //    Hand = rightHandTransform.gameObject;
            //}
            //else
            //{
            //    Debug.LogError("RightHand không được tìm thấy! Kiểm tra cấu trúc Player.");
            //}
        }
    }

    void Update()
    {
        if (!onhand)
        {
            onhand = photograph.GetComponent<OnHand>().onHand;
        }

        if (onhand == true && photoReach != null)
        {
            photoReach.SetActive(true);
        }

        if (photonView.IsMine && PlayerInZone && onhand && Input.GetKeyDown(KeyCode.F) && !_photoCD.IsOutOfUseTime)
        {
            if (!isActive)
            {
                StartCoroutine(ActivateFlashlightForOneSecond());
                photonView.RPC("PlaySound", RpcTarget.All); // Phát âm thanh đồng bộ
            }
        }

        if (photonView.IsMine)
        {
            _photoCD.UseTimeUpdate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            PlayerInZone = true;
        }
    }

    [PunRPC]
    private void PlaySound()
    {
        if (audioSource != null && sound1 != null)
        {
            audioSource.clip = sound1;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource hoặc AudioClip bị thiếu!");
        }
    }

    [PunRPC]
    private void SyncFlashlight(bool state)
    {
        Plight.SetActive(state);
        isActive = state;
    }

    IEnumerator ActivateFlashlightForOneSecond()
    {
        if (Hand != null)
        {
            Hand.GetComponent<PhotonEquip>().enabled = false;
        }

        isActive = true;
        photonView.RPC("SyncFlashlight", RpcTarget.All, true); // Đồng bộ bật đèn

        yield return new WaitForSeconds(1f);

        photonView.RPC("SyncFlashlight", RpcTarget.All, false); // Đồng bộ tắt đèn

        isActive = false;
        if (Hand != null)
        {
            Hand.GetComponent<PhotonEquip>().enabled = true;
        }

        if (photonView.IsMine)
        {
            _photoCD.DecreaseFlsUseTimes();
        }
    }
}
