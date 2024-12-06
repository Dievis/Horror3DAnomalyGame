using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTriggerNumber : MonoBehaviour
{
    public int TriggersNumber;
    [SerializeField] private Cooldown _cooldown; // cach so giay nhat dinh se check xem co vuot qua anomaly khong 
    public GameObject TriggerAction; // Chinh thanh jumpscare sao cho phu hop
    [Header("readonly")]
    public int TriggersCount;
    public void Increase(){TriggersCount++;}
    public void Decrease() { TriggersCount--;}

    public void Update()
    {
        if (_cooldown.IsCoolingDown) return;
        if(TriggersCount == TriggersNumber)
        {
            if(TriggerAction != null)
            TriggerAction.SetActive(true);// Chinh thanh jumpscare sao cho phu hop
            Debug.Log("Kich hoat dieu kien thua");
            this.enabled = false;
        }
        _cooldown.StartCooldown();
    }
}
