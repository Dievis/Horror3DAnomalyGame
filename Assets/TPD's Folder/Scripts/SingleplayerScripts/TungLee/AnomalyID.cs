using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnomalyID: MonoBehaviour
{
    public GameObject Anomaly;
    public Changed ch;
    // Start is called before the first frame update
    void Awake()
    {
        ch = Anomaly.GetComponent<Changed>();
        ch.enabled = false;
    }
    public void Active()
    {
        ch = Anomaly.GetComponent<Changed>();
        ch.enabled = true;
    }
}
