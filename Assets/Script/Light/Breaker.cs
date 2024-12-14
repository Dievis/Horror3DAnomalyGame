using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Breaker : MonoBehaviour
{
    public bool powerSource;
    public AudioSource audioSource; // Add this
    public AudioClip sound1; // Add this
    //public GameObject txtToDisplay;
    private bool PlayerInZone;
    // Start is called before the first frame update
    void Start()
    {
        PlayerInZone = false;                   //player not in zone       
        //txtToDisplay.SetActive(false);
        powerSource = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerInZone && Input.GetKeyDown(KeyCode.E))           //if in zone and press E key
        {
            powerSource = !powerSource;
            audioSource.clip = sound1; // Add this
            audioSource.Play(); // Add this
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")  //if player in zone
        {
            //txtToDisplay.SetActive(true);
            PlayerInZone = true;
        }
    }

    private void OnTriggerExit(Collider other) //if player exit zone
    {
        if (other.gameObject.tag == "Reach")
        {
            PlayerInZone = false;
            //txtToDisplay.SetActive(false);
        }
    }
}
