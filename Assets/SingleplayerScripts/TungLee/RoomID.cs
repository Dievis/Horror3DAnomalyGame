using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomID : MonoBehaviour
{
    public GameObject room;
    public GameObject Switch;
    public AnomalyID[] Anomalys;
    public OpenCloseDoor1 doorscript;
    public int lastValue;
    public int NumOfAnomaly;
    public int AnomalyDifficultLevel;
    public int NumberOfAnomalyTrigger;
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("started");
        Anomalys = room.GetComponentsInChildren<AnomalyID>();
        NumOfAnomaly = Anomalys.Length;
        NumberOfAnomalyTrigger = 1;
    }
    public void RoomTrigger()
    {
        if (doorscript != null)
            if (!doorscript.toggle == false)
                doorscript.DoorCloseed();
        Switch.GetComponent<LightOnOffNew>().IsPower = false;
        for (int i = 0; i < NumberOfAnomalyTrigger; i++)
        {
            int random = Random.Range(0, Anomalys.Length);
            // chỗ này để random, đg test nên để 0 là cái đầu tiên (nhiều thì thành random)
            Anomalys[random].Active();
        }
    }
}