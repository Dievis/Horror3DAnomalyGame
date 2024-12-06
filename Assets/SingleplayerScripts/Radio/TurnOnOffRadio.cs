using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class TurnOnOffRadio : MonoBehaviour
{
    public GameObject txtToDisplay;             //display the UI text
    private bool PlayerInZone;
    public AudioSource audioSource;        //audio source
    public AudioClip OnRadio;            //sound 1
    public AudioClip OffRadio;
    public AudioClip turtorial;
    public bool IsOpened;



    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ActivateRaidoAfter20Second());
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerInZone && Input.GetKeyDown(KeyCode.E))           //if in zone and press E key
        {
            IsOpened = !IsOpened;
            opencloseRadio();
        }
        
    }
    void opencloseRadio()
    {
            if (IsOpened == true)
            {
                audioSource.PlayOneShot(OnRadio);
                audioSource.clip = turtorial;
                audioSource.Play();
            } 
            else 
            {
                audioSource.clip = turtorial;
                audioSource.Stop();
                audioSource.PlayOneShot(OffRadio);
            }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")  //if player in zone
        {
            txtToDisplay.SetActive(true);
            PlayerInZone = true;
        }
    }


    private void OnTriggerExit(Collider other)     //if player exit zone
    {
        if (other.gameObject.tag == "Reach")
        {
            PlayerInZone = false;
            txtToDisplay.SetActive(false);
        }
    }

    IEnumerator ActivateRaidoAfter20Second()
    {
        IsOpened = false;

        yield return new WaitForSeconds(20f);

        IsOpened = true;
        opencloseRadio();
    }

}
