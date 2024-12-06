using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerLightOff : MonoBehaviour
{
    public RandomTimeTrigger anomalyStartTime;
    void Start()
    {
        anomalyStartTime.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hour")
        {
            Debug.Log("12 gio xe dap di tren pho!");
            anomalyStartTime.enabled = true;
        }
    }
}
