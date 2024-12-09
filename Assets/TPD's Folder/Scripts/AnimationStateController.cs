using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;
    float VelocityZ = 0.0f;
    float VelocityX = 0.0f;
    public float acceleration = 2.0f;
    public float deceleration = 2.0f;
    
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool forwardPressed = Input.GetKey("w");
        bool leftPressed = Input.GetKey("a");
        bool rightPressed = Input.GetKey("d");
        bool runPressed = Input.GetKey("left shift");

        //Di chuyen ve phia trc va nhan shift phai de tang toc
        if (forwardPressed && VelocityZ < 0.5f && !runPressed)
        {
            VelocityZ += Time.deltaTime * acceleration;
        }

        //Di chuyen ve phia trai va nhan shift phai de tang toc
        if (leftPressed && VelocityX < -0.5f && !runPressed)
        {
            VelocityX -= Time.deltaTime * acceleration;
        }

        //Di chuyen ve phia phai va nhan shift phai de tang toc
        if (rightPressed && VelocityX < 0.5f && !runPressed)
        {
            VelocityX += Time.deltaTime * acceleration;
        }

        //Di chuyen ve phia sau
        if (!forwardPressed && VelocityZ > 0.0f)
        {
            VelocityZ -= Time.deltaTime * deceleration;
        }

        //Lam moi van toc Z
        if (!forwardPressed && VelocityZ < 0.0f)
        {
            VelocityZ = 0.0f;
        }

        //Tang van toc X neu khong nhan ben trai va van toc X < 0 
        if (!leftPressed && VelocityX < 0.0f)
        {
            VelocityX += Time.deltaTime * deceleration;
        }

        //Tang van toc X neu khong nhan ben trai va van toc X > 0 
        if (!rightPressed && VelocityX > 0.0f)
        {
            VelocityX -= Time.deltaTime * deceleration;
        }

        //Lam moi van toc X
        if (!rightPressed && !leftPressed && VelocityX != 0.0f && (VelocityX > -0.5f && VelocityX < 0.5f))
        {
            VelocityX = 0.0f;
        }

        animator.SetFloat("VelocityZ", VelocityZ);
        animator.SetFloat("VelocityX", VelocityX);
    }
}
