using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // Sử dụng Photon

public class PhotonEquip : MonoBehaviourPunCallbacks
{
    public EquipSlot Slots;         // Các slot trang bị
    public GameObject CurrentObj;   // Đối tượng hiện tại được kích hoạt

    void Start()
    {
        if (!photonView.IsMine) return; // Chỉ thực hiện trên player của chính mình

        Transform rightHand = transform.FindRecursive("RightHand"); // Tìm "RightHand" trong chính GameObject của player

        if (rightHand == null)
        {
            Debug.LogError("RightHand không tìm thấy!");
            return;
        }

        Slots = rightHand.GetComponent<EquipSlot>();
        if (Slots == null)
        {
            Debug.LogError("EquipSlot component không tìm thấy trong RightHand!");
            return;
        }

        CurrentObj = null; // Đặt null vì ban đầu chưa có vật phẩm nào được trang bị
    }

    void Update()
    {
        if (!photonView.IsMine) return; // Chỉ thực hiện cho player của chính mình

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipItem(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipItem(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EquipItem(2);
        }
    }

    void EquipItem(int slotIndex)
    {
        if (CurrentObj != null)
        {
            CurrentObj.SetActive(false); // Tắt đối tượng hiện tại
        }

        if (Slots.ItemInSlot[slotIndex] != null)
        {
            CurrentObj = Slots.ItemInSlot[slotIndex];
            photonView.RPC("RPC_SetActiveItem", RpcTarget.AllBuffered, slotIndex); // Gọi RPC để đồng bộ
        }
    }

    [PunRPC]
    void RPC_SetActiveItem(int slotIndex)
    {
        // Đồng bộ thay đổi cho tất cả client
        if (Slots.ItemInSlot[slotIndex] != null)
        {
            if (CurrentObj != null)
            {
                CurrentObj.SetActive(false); // Tắt đối tượng hiện tại
            }

            CurrentObj = Slots.ItemInSlot[slotIndex];
            CurrentObj.SetActive(true); // Kích hoạt đối tượng mới
        }
    }
}