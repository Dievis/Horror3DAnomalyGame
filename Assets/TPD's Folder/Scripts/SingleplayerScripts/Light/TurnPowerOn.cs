using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurnPowerOn : MonoBehaviour
{
    public GameObject[] lights;

    public GameObject text;

    public bool powerIsOn;

    private bool inReach;


    void Start()
    {
        
        foreach (GameObject ob in lights)
        {
            ob.SetActive(false);
        }
        inReach = false;                   //player not in zone       
        text.SetActive(false);  
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !powerIsOn)
        {
            inReach = true;
            text.SetActive(true);
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inReach = false;
            text.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && inReach)
        {
            powerIsOn = true;
        }

        if(powerIsOn)
        {
            foreach (GameObject ob in lights)
            {
                ob.SetActive(true);
            }

            inReach = false;
            text.SetActive(false);
        }

        if (!powerIsOn)
        {
            foreach (GameObject ob in lights)
            {
                ob.SetActive(false);
            }
        }
    }
}
