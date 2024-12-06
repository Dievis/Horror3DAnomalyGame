using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpPhotograph : MonoBehaviour
{
    public GameObject text;
    public GameObject PhotographtOnPlayer;

    // Start is called before the first frame update
    void Start()
    {
        PhotographtOnPlayer.SetActive(false);
        text.SetActive(false);
    }
    private void OnTriggerStay(Collider other)
    {
        text.SetActive(true);
        if (other.gameObject.tag == "Player")
        {
            if (Input.GetKey(KeyCode.E))
            {
                this.gameObject.SetActive(false);
                PhotographtOnPlayer.SetActive(true);
                text.SetActive(false);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        text.SetActive(false);
    }

}
