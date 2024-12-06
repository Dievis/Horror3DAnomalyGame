using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InteractableSwitchLight : MonoBehaviourPunCallbacks
{
    public GameObject txtToDisplay; // Display the UI text
    private bool PlayerInZone; // Check if the player is in trigger
    public AudioSource audioSource; // Add this
    public AudioClip sound1; // Add this
    public GameObject switchON, switchOFF;
    public GameObject lightorobj, breaker;

    public bool IsPower, IsBreaker;

    void Start()
    {
        breaker = GameObject.FindGameObjectWithTag("Breaker"); // Find Breaker with tag "Breaker"
        IsPower = true;
        PlayerInZone = false; // Player not in zone       
        txtToDisplay.SetActive(false);
        switchON.SetActive(true);
        switchOFF.SetActive(false);
    }

    void Update()
    {
        if (PlayerInZone && Input.GetKeyDown(KeyCode.E)) // If in zone and press E key
        {
            photonView.RPC("ToggleLight", RpcTarget.All); // Call RPC to sync across players
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach") // If player in zone
        {
            txtToDisplay.SetActive(true);
            PlayerInZone = true;
        }
    }

    private void OnTriggerExit(Collider other) // If player exits zone
    {
        if (other.gameObject.tag == "Reach")
        {
            PlayerInZone = false;
            txtToDisplay.SetActive(false);
        }
    }

    public void LightOnOff(bool powersource)
    {
        // Check if breaker is assigned before using it
        if (breaker == null)
        {
            breaker = GameObject.FindGameObjectWithTag("Breaker");
            if (breaker == null)
            {
                Debug.LogError("Breaker không được tìm thấy! Kiểm tra tag hoặc sự tồn tại của GameObject.");
                return;
            }
        }

        // Check if Breaker component exists
        InteractableBreaker breakerComponent = breaker.GetComponent<InteractableBreaker>();
        if (breakerComponent == null)
        {
            Debug.LogError("Component Interactable Breaker không tồn tại trên đối tượng Breaker.");
            return;
        }

        IsBreaker = breakerComponent.powerSource;

        // Turn on or off the light based on power source state
        if (IsBreaker)
        {
            lightorobj.SetActive(powersource);
        }
        else
        {
            lightorobj.SetActive(false);
        }
    }

    [PunRPC]
    void ToggleLight()
    {
        IsPower = !IsPower; // Toggle power state
        switchON.SetActive(!switchON.activeSelf);
        switchOFF.SetActive(!switchOFF.activeSelf);
        audioSource.clip = sound1; // Play sound
        audioSource.Play();
        LightOnOff(IsPower); // Update light state
    }
}
