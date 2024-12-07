using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun; // Thêm Photon.Pun

public class InteractableItem : MonoBehaviourPunCallbacks
{
    public GameObject inttext, Item;
    public AudioSource pickup;
    public AudioClip pickupSound;
    public Transform hand;
    public Vector3 pos;
    public Quaternion rot;
    public bool inReach;
    public bool followhand;
    public EquipSlot Slots;
    public ReachItem CurrentItem;

    private PhotonView photonView; // Thêm PhotonView

    void Start()
    {
        // Khởi tạo PhotonView
        photonView = GetComponent<PhotonView>();

        // Đảm bảo rằng chỉ thực hiện nếu đây là object của người chơi hiện tại
        if (!photonView.IsMine)
        {
            return;
        }

        // Tìm kiếm GameObject của player hiện tại
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player GameObject with tag 'Player' not found!");
            return;
        }

        Transform rightHand = player.transform.FindRecursive("RightHand");
        if (rightHand == null)
        {
            Debug.LogError("RightHand Transform không tồn tại bên trong Player hoặc các thành phần con của nó!");
            return;
        }

        hand = rightHand;

        // Gán giá trị ban đầu cho pos
        pos = hand.transform.position;

        // Đảm bảo EquipSlot được gán từ RightHand
        Slots = rightHand.GetComponent<EquipSlot>();
        if (Slots == null)
        {
            Debug.LogError("EquipSlot component is missing from the RightHand GameObject.");
        }

        //// Đảm bảo ReachItem được tìm thấy
        //CurrentItem = GameObject.FindGameObjectWithTag("Reach")?.GetComponent<ReachItem>();
        //if (CurrentItem == null)
        //{
        //    Debug.LogError("ReachItem component is missing from the Reach GameObject.");
        //}

        // Đảm bảo AudioSource tồn tại
        pickup = GetComponent<AudioSource>();
        if (pickup == null)
        {
            Debug.LogError("AudioSource component is missing from the GameObject.");
        }

        followhand = false;
    }  

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inttext.SetActive(true);
            inReach = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inttext.SetActive(false);
            inReach = false;
        }
    }

    void Update()
    {
        if (inReach == true && !followhand)
        {
            if (Input.GetKeyDown(KeyCode.E) && photonView.IsMine) // Kiểm tra quyền sở hữu
            {
                if (GetItemIntoSlot())
                {
                    photonView.RPC("PickUpItem", RpcTarget.All); // Đồng bộ nhặt item
                }
            }
        }
        else if (followhand)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && photonView.IsMine) // Kiểm tra quyền sở hữu
            {
                photonView.RPC("SetItemActive", RpcTarget.All, true);
            }
            transform.position = hand.position;
            transform.rotation = hand.rotation;
        }
    }

    private bool GetItemIntoSlot()
    {
        for (int i = 0; i < Slots.ItemInSlot.Length; i++)
        {
            if (!Slots.IsFull[i])
            {
                Slots.ItemInSlot[i] = Item;
                Slots.IsFull[i] = true;
                return true;
            }
        }
        return false;
    }

    void PlayPickUpSound()
    {
        if (pickup != null && pickupSound != null)
        {
            pickup.PlayOneShot(pickupSound);
        }
        else
        {
            Debug.LogWarning("Pickup sound or AudioSource is missing.");
        }
    }

    // RPC để nhặt item
    [PunRPC]
    void PickUpItem()
    {
        inttext.SetActive(false);
        inReach = false;
        PlayPickUpSound();
        Item.transform.localPosition = pos;
        followhand = true;
        OnHand temp = CurrentItem?.CurrentItem.GetComponent<OnHand>(); // Check null for CurrentItem
        if (temp != null)
        {
            temp.onHand = followhand;
        }
        Item.SetActive(false);
    }

    // RPC để bật/tắt item
    [PunRPC]
    void SetItemActive(bool isActive)
    {
        Item.SetActive(isActive);
    }
}
