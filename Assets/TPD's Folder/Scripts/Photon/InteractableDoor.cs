using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InteractableDoor : MonoBehaviourPun, IPunObservable
{
    public GameObject txtDoor;             // Hiển thị UI text
    public AudioSource audioSource;        // Nguồn âm thanh
    public AudioClip openSound;            // Âm thanh mở cửa
    public AudioClip closeSound;           // Âm thanh đóng cửa
    public Animator door;                  // Animator của cửa

    private bool inReach;                  // Kiểm tra người chơi có trong vùng kích hoạt không
    [SerializeField] private bool isOpen;  // Trạng thái cửa (đóng/mở)

    void Start()
    {
        inReach = false;
        txtDoor.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            txtDoor.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = false;
            txtDoor.SetActive(false);
        }
    }

    void Update()
    {
        if (inReach && Input.GetKeyDown(KeyCode.E)) // Người chơi trong vùng và nhấn phím E
        {
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("ToggleDoor", RpcTarget.AllBuffered);
            }
            else
            {
                ToggleDoor();
            }
        }
    }

    [PunRPC]
    public void ToggleDoor()
    {
        if (isOpen)
        {
            DoorCloseed();
        }
        else
        {
            DoorOpened();
        }
    }

    public void DoorOpened()
    {
        door.ResetTrigger("closed");
        door.SetTrigger("opened");
        audioSource.PlayOneShot(openSound);
        isOpen = true;
        txtDoor.SetActive(false);
    }

    public void DoorCloseed()
    {
        door.ResetTrigger("opened");
        door.SetTrigger("closed");
        audioSource.PlayOneShot(closeSound);
        isOpen = false;
        txtDoor.SetActive(false);
    }

    // Đồng bộ hóa trạng thái cửa qua mạng
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isOpen);
        }
        else if (stream.IsReading)
        {
            isOpen = (bool)stream.ReceiveNext();
            if (isOpen)
            {
                DoorOpened();
            }
            else
            {
                DoorCloseed();
            }
        }
    }
}
