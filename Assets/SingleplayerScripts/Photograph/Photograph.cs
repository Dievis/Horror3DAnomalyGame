using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Photograph : MonoBehaviour
{
    public GameObject photograph; 
    public GameObject Plight;
    public AudioSource audioSource; // Add this
    public AudioClip sound1; // Add this
    private bool PlayerInZone;
    private bool isActive = false;
    private GameObject Hand;
    public bool onhand;
    public GameObject photoReach;
    [SerializeField] public PhotoCD _photoCD;

    void Start()
    {
        photograph.SetActive(true);
        Plight.SetActive(false);
        PlayerInZone = false;
        onhand = false;
        photoReach.SetActive(false);
        Hand = GameObject.FindGameObjectWithTag("RightHand");
    }

    void Update()
    {
        if (!onhand)
        {
            onhand = photograph.GetComponent<OnHand>().onHand;

        }
        if (onhand == true)
        {
            photoReach.SetActive(true);
        }
        if (PlayerInZone && onhand && Input.GetKeyDown(KeyCode.F) && !_photoCD.IsOutOfUseTime)           //if in zone and press E key
        {
            if (!isActive)
            {
                StartCoroutine(ActivateFlashlightForOneSecond());
                audioSource.clip = sound1; // Add this
                audioSource.Play(); // Add this
            }
        }
        _photoCD.UseTimeUpdate();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")     //if player in zone
        {
            PlayerInZone = true;
        }
    }

    IEnumerator ActivateFlashlightForOneSecond()
    {

        Hand.GetComponent<Equip>().enabled = false;
        isActive = true;
        Plight.SetActive(true);

        yield return new WaitForSeconds(1f);

        Plight.SetActive(false);
        isActive = false;
        Hand.GetComponent<Equip>().enabled = true;
        _photoCD.DecreaseFlsUseTimes();
    }
}
