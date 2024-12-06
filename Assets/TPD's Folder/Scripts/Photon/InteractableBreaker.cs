using System.Collections;
using UnityEngine;
using Photon.Pun;

public class InteractableBreaker : MonoBehaviourPunCallbacks
{
    public bool powerSource;
    public AudioSource audioSource; // Audio source for sound effects
    public AudioClip sound1; // Sound to play when toggling power
    private bool PlayerInZone; // Check if the player is in trigger zone

    void Start()
    {
        PlayerInZone = false; // Player not in zone       
        powerSource = true; // Power is on by default
    }

    void Update()
    {
        if (PlayerInZone && Input.GetKeyDown(KeyCode.E)) // If in zone and press E key
        {
            photonView.RPC("TogglePowerSource", RpcTarget.All); // Call RPC to sync power state
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach") // If player enters the zone
        {
            PlayerInZone = true;
        }
    }

    private void OnTriggerExit(Collider other) // If player exits the zone
    {
        if (other.gameObject.tag == "Reach")
        {
            PlayerInZone = false;
        }
    }

    [PunRPC]
    void TogglePowerSource()
    {
        powerSource = !powerSource; // Toggle power source state
        audioSource.clip = sound1; // Play the sound effect
        audioSource.Play();
    }
}
