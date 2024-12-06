using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurnPowerOn1 : MonoBehaviour
{
    public GameObject[] lights;

    public GameObject txtToDisplay;             //display the UI text

    private bool PlayerInZone;                  //check if the player is in trigger

    public bool powerIsOn;

    private void Start()
    {
        foreach (GameObject ob in lights)
        {
            ob.SetActive(false);
        }       
        PlayerInZone = false;                   //player not in zone       
        txtToDisplay.SetActive(false);
    }
    private void Update()
    {
        if (PlayerInZone && Input.GetKeyDown(KeyCode.E))           //if in zone and press E key
        {

            foreach (GameObject ob in lights)
            {
                ob.SetActive(!ob.activeSelf);
            }          
            gameObject.GetComponent<AudioSource>().Play();
            gameObject.GetComponent<Animator>().Play("switch");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !powerIsOn)     //if player in zone
        {
            txtToDisplay.SetActive(true);
            PlayerInZone = true;
        }
    }


    private void OnTriggerExit(Collider other)     //if player exit zone
    {
        if (other.gameObject.tag == "Player" )
        {
            PlayerInZone = false;
            txtToDisplay.SetActive(false);
        }
    }
}
