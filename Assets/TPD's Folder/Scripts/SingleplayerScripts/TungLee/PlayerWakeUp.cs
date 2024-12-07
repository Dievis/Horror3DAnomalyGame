using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWakeUp : MonoBehaviour
{

    public Animator animatorToPlay;


    private void Awake()
    {
        if (animatorToPlay != null)
        {
            animatorToPlay.Play("Player");
        }
    }

}
