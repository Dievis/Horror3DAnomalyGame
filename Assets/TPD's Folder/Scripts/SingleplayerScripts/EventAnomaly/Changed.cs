using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Changed : MonoBehaviour
{
    public GameObject normal;
    public GameObject anomaly;
    void Update()
    {
        LightOff();
    }
    public void LightOff()
    {
        normal.SetActive(false);
        anomaly.SetActive(true);
    }
}
