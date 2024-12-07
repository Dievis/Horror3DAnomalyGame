using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equip : MonoBehaviour
{
    public EquipSlot Slots;
    public GameObject CurrentObj;
    public void Start()
    {
        Slots = GameObject.FindGameObjectWithTag("RightHand").GetComponent<EquipSlot>();
        CurrentObj = new GameObject();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (CurrentObj != null)
                CurrentObj.SetActive(false);
            if (Slots.ItemInSlot[0] != null)
            {
                CurrentObj = Slots.ItemInSlot[0];
                CurrentObj.SetActive(true);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (CurrentObj != null)
                CurrentObj.SetActive(false);
            if (Slots.ItemInSlot[1] != null)
            {
                CurrentObj = Slots.ItemInSlot[1];
                CurrentObj.SetActive(true);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (CurrentObj != null)
                CurrentObj.SetActive(false);
            if (Slots.ItemInSlot[2] != null)
            {
                CurrentObj = Slots.ItemInSlot[2];
                CurrentObj.SetActive(true);
            }
        }
    }
}



