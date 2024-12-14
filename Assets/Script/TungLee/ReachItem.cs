using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReachItem : MonoBehaviour
{
    public GameObject CurrentItem;
    public bool check;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Item"))
        {
            check = true;
            CurrentItem = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        check = false;
        CurrentItem = null;
    }
}
