using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurnToNormal : MonoBehaviour
{
    public AudioSource test;
    public AudioClip sound;
    public GameObject txtToDisplay;             //display the UI text
    public bool inCameraReach;
    public Changed ch;
    public GameObject Camera;
    public GameObject anomaly;
    public bool ena;
    [SerializeField] private PhotoCD _photoCD;
    public SetTriggerNumber triggerNumber;

    void Start()
    {
        inCameraReach = false;
        txtToDisplay.SetActive(false);
        ena = false;

        if (Camera != null)
        {
            Photograph photoComponent = Camera.GetComponent<Photograph>();
            if (photoComponent == null)
            {
                Debug.LogError("Photograph component not found on Camera.");
            }
            else
            {
                _photoCD = photoComponent._photoCD;
                if (_photoCD == null)
                {
                    Debug.LogError("PhotoCD is not initialized in Photograph.");
                }
            }
        }

        if (triggerNumber == null)
        {
            triggerNumber = GameObject.Find("AllAnomalys").GetComponent<SetTriggerNumber>();
            if (triggerNumber == null)
            {
                Debug.LogError("SetTriggerNumber component not found.");
            }
        }

        if (ch == null)
        {
            ch = GetComponent<Changed>(); // Or assign it manually
            if (ch == null)
            {
                Debug.LogError("Changed component not found.");
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        CameraContact();

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "CameraReach")  //if player in zone
        {
            txtToDisplay.SetActive(true);
            inCameraReach = true;
        }
    }


    private void OnTriggerExit(Collider other)     //if player exit zone
    {
        if (other.gameObject.tag == "CameraReach")
        {
            inCameraReach = false;
            txtToDisplay.SetActive(false);
        }
    }

    public void CameraContact()
    {
        if (Camera == null)
        {
            Debug.LogError("Camera is not assigned.");
            return;
        }

        if (_photoCD == null)
        {
            Debug.LogError("_photoCD is not assigned or initialized properly.");
            return;
        }

        if (Camera.activeSelf)
        {
            ena = true;

            if (inCameraReach && Input.GetKeyDown(KeyCode.F) && !_photoCD.IsOutOfUseTime)
            {
                if (ch == null || anomaly == null)
                {
                    Debug.LogError("Either 'ch' or 'anomaly' is null.");
                    return;
                }

                ch.enabled = false;
                anomaly.SetActive(false);

                if (ch.normal != null)
                {
                    ch.normal.SetActive(true);
                }
                else
                {
                    Debug.LogError("'ch.normal' is null.");
                }

                if (test != null && sound != null)
                {
                    test.PlayOneShot(sound);
                }
                else
                {
                    Debug.LogError("'test' or 'sound' is not assigned.");
                }

                inCameraReach = false;
                if (txtToDisplay != null)
                {
                    txtToDisplay.SetActive(false);
                }

                if (triggerNumber != null)
                {
                    triggerNumber.Decrease();
                }
                else
                {
                    Debug.LogError("'triggerNumber' is null.");
                }

                AnomalyCount anomalyCount = gameObject.GetComponent<AnomalyCount>();
                if (anomalyCount == null)
                {
                    anomalyCount = gameObject.AddComponent<AnomalyCount>();
                }
                anomalyCount.enabled = true;
            }
        }
        else
        {
            ena = false;
        }
    }

}

