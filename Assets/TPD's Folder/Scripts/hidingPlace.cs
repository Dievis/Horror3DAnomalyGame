using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hidingPlace : MonoBehaviour
{
    public GameObject hideText, stopHideText;
    public GameObject normalPlayer, hidingPlayer;
    public EnemyAI monsterScript;
    public Transform monsterTransform;
    public float loseDistance;

    private bool interactable = false;
    private bool hiding = false;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Reach") && !hiding)
        {
            hideText.SetActive(true);
            interactable = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            hideText.SetActive(false);
            interactable = false;
        }
    }

    void Update()
    {
        // Check for hiding interaction
        if (interactable && Input.GetKeyDown(KeyCode.E) && !hiding)
        {
            StartHiding();
        }
        // Check for unhiding interaction
        else if (hiding && Input.GetKeyDown(KeyCode.E))
        {
            StopHiding();
        }
    }

    void StartHiding()
    {
        hideText.SetActive(false);
        hidingPlayer.SetActive(true);

        // Check distance and stop chase if necessary
        float distance = Vector3.Distance(monsterTransform.position, normalPlayer.transform.position);
        if (distance > loseDistance && monsterScript.chasing)
        {
            monsterScript.stopChase();
        }

        stopHideText.SetActive(true);
        hiding = true;
        normalPlayer.SetActive(false);
    }

    void StopHiding()
    {
        stopHideText.SetActive(false);
        normalPlayer.SetActive(true);
        hidingPlayer.SetActive(false);
        hiding = false;
    }
}
