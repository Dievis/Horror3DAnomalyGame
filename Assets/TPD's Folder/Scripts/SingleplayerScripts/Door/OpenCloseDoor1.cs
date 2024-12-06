using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCloseDoor1 : MonoBehaviour
{
    public GameObject txtDoor;             //display the UI text
    public AudioSource audioSource;        //audio source
    public AudioClip openSound;            //sound 1
    public AudioClip closeSound;           //sound 2
    public bool inReach, toggle;
    //check if the player is in trigger

    public Animator door;
    // Start is called before the first frame update
    void Start()
    {
        inReach = false;                   //player not in zone       
        toggle = true;
        txtDoor.SetActive(false);
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Reach")     //if player in zone
        {
            inReach = true;
            txtDoor.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")     //if player in zone
        {
            inReach = false;
            txtDoor.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        openclosedoor();
    }
    void openclosedoor()
    {
        if (inReach && Input.GetKeyDown(KeyCode.E))           //if in zone and press E key
        {
            if (toggle) DoorOpened();
            else DoorCloseed();
            txtDoor.SetActive(false);
            inReach = false;
        }
    }
    public void DoorOpened()
    {
        door.ResetTrigger("closed");
        door.SetTrigger("opened");
        audioSource.PlayOneShot(openSound);
        toggle = false;
    }
    public void DoorCloseed()
    {
        door.ResetTrigger("opened");
        door.SetTrigger("closed");
        audioSource.PlayOneShot(closeSound);  //play close sound
        toggle = true;
    }
}
