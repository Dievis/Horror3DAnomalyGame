using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpFlashLight : MonoBehaviour
{
    public GameObject text;
    public GameObject FlashLightOnPlayer;

    // Start is called before the first frame update
    void Start()
    {
        FlashLightOnPlayer.SetActive(false);
        text.SetActive(false);
    }
    private void OnTriggerStay(Collider other)
    {
        text.SetActive(true);
        if (other.gameObject.tag == "Player")
        {
            if(Input.GetKey(KeyCode.E)) 
            {
                this.gameObject.SetActive(false);
                FlashLightOnPlayer.SetActive(true);
                text.SetActive(false);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        text.SetActive(false);  
    }

}
