using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EquipSlot : MonoBehaviour
{
    public GameObject[] ItemInSlot;
    public bool[] IsFull;
    public void Awake()
    {
        ItemInSlot = new GameObject[3];
        IsFull = new bool[3];
    }
}

