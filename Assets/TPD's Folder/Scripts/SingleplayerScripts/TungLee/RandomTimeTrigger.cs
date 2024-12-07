using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTimeTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    //time between 2 trigger function ( second)
    [SerializeField]  private Cooldown _cooldown;
    public GameObject AllAnomalys;
    public RoomID[] rooms;
    public int trigger;
    public int triggerRate;
    void Awake()
    {
        Debug.Log("started time trigger");
        rooms = AllAnomalys.GetComponentsInChildren<RoomID>();
        //triggerRate = 5;
    }
    // Update is called once per frame
    void Update()
    {
        if (_cooldown.IsCoolingDown) return;
        else
        {
            trigger = Random.Range(0, 100);
            if (trigger <= triggerRate)
            {

                int random = Random.Range(0, rooms.Length);
                // chỗ này để random, đg test nên để 0 là cái đầu tiên 
                rooms[random].RoomTrigger();
            }
            _cooldown.StartCooldown();
        }    
    }
}
