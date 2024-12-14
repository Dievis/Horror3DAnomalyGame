using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTimer : MonoBehaviour
{
    public float CountDownTime;
    public float CurrentTime;
    public GameObject TriggerAnomaly;
    public GameObject TriggerAction;
    private bool _UpdateToggle;
    void Start()
    {
        CurrentTime = CountDownTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (TriggerAnomaly.activeSelf)
        {
            CurrentTime -= Time.deltaTime;
            _UpdateToggle = true;
            if(CurrentTime <= 0)
            {
                //if(TriggerAction!=null)                         //co obj thi xoa //
                {
                    //TriggerAction.SetActive(true);               //co obj thi xoa //
                    CurrentTime = CountDownTime; // se con sua thuy thuoc vao anomaly
                    Debug.Log("Kich hoat dieu kien thua");
                }
            }
        }
        else if(_UpdateToggle) 
        {
            CurrentTime = CountDownTime;
            _UpdateToggle = false;
        }
    }
}
