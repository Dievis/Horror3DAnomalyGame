using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOnOf : MonoBehaviour
{
    public GameObject txtToDisplay;             //display the UI text

    private bool inReach;                  //check if the player is in trigger

    public GameObject lightorobj;

    private void Start()
    {

        inReach = false;                   //player not in zone       
        txtToDisplay.SetActive(false);
    }
    private void Update()
    {
        if (inReach && Input.GetKeyDown(KeyCode.E))           //if in zone and press E key
        {
            lightorobj.SetActive(!lightorobj.activeSelf);
            gameObject.GetComponent<AudioSource>().Play();
            gameObject.GetComponent<Animator>().Play("switch");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")  //if player in zone
        {
            txtToDisplay.SetActive(true);
            inReach = true;
        }
    }


    private void OnTriggerExit(Collider other)     //if player exit zone
    {
        if (other.gameObject.tag == "Player")
        {
            inReach = false;
            txtToDisplay.SetActive(false);
        }
    }
}