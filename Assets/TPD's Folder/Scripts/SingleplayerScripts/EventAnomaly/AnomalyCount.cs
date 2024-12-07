using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnomalyCount : MonoBehaviour
{
    public SetTriggerNumber triggerNumber;
    void Start()
    {
        triggerNumber = GameObject.Find("AllAnomalys").GetComponent<SetTriggerNumber>();
    }
    // Update is called once per frame
    void Update()
    {
        triggerNumber.Increase();
        this.enabled = false;
    }
}
