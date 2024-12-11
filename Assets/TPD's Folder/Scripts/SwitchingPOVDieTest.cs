//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class AnimationStateController : MonoBehaviour
//{
//    Animator animator;
//    float VelocityZ = 0.0f;
//    float VelocityX = 0.0f;
//    public float acceleration = 2.0f;
//    public float deceleration = 2.0f;

//    void Start()
//    {
//        animator = GetComponent<Animator>();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        bool forwardPressed = Input.GetKey("w");
//        bool leftPressed = Input.GetKey("a");
//        bool rightPressed = Input.GetKey("d");
//        bool runPressed = Input.GetKey("left shift");

//        //Di chuyen ve phia trc va nhan shift phai de tang toc
//        if (forwardPressed && VelocityZ < 0.5f && !runPressed)
//        {
//            VelocityZ += Time.deltaTime * acceleration;
//        }

//        //Di chuyen ve phia trai va nhan shift phai de tang toc
//        if (leftPressed && VelocityX < -0.5f && !runPressed)
//        {
//            VelocityX -= Time.deltaTime * acceleration;
//        }

//        //Di chuyen ve phia phai va nhan shift phai de tang toc
//        if (rightPressed && VelocityX < 0.5f && !runPressed)
//        {
//            VelocityX += Time.deltaTime * acceleration;
//        }

//        //Di chuyen ve phia sau
//        if (!forwardPressed && VelocityZ > 0.0f)
//        {
//            VelocityZ -= Time.deltaTime * deceleration;
//        }

//        //Lam moi van toc Z
//        if (!forwardPressed && VelocityZ < 0.0f)
//        {
//            VelocityZ = 0.0f;
//        }

//        //Tang van toc X neu khong nhan ben trai va van toc X < 0 
//        if (!leftPressed && VelocityX < 0.0f)
//        {
//            VelocityX += Time.deltaTime * deceleration;
//        }

//        //Tang van toc X neu khong nhan ben trai va van toc X > 0 
//        if (!rightPressed && VelocityX > 0.0f)
//        {
//            VelocityX -= Time.deltaTime * deceleration;
//        }

//        //Lam moi van toc X
//        if (!rightPressed && !leftPressed && VelocityX != 0.0f && (VelocityX > -0.5f && VelocityX < 0.5f))
//        {
//            VelocityX = 0.0f;
//        }

//        animator.SetFloat("VelocityZ", VelocityZ);
//        animator.SetFloat("VelocityX", VelocityX);
//    }
//}

using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwitchingPOVDieTest : MonoBehaviourPunCallbacks
{
    [Header("UI Elements")]
    public Camera playerCamera; // Camera của người chơi
    public TMP_Text spectateText; // UI Text để hiển thị thông báo
    private bool isDead = false;

    void Start()
    {
        if (photonView.IsMine && spectateText != null)
        {
            spectateText.gameObject.SetActive(false); // Tắt thông báo ban đầu
        }
    }

    void Update()
    {
        // Giả sử "Q" là phím để kiểm tra chết
        if (Input.GetKeyDown(KeyCode.Q) && photonView.IsMine && !isDead)
        {
            Die();
        }
    }

    public void Die()
    {
        if (!photonView.IsMine) return;

        isDead = true;

        // Tắt camera của người chơi
        playerCamera.enabled = false;

        // Gửi yêu cầu chuyển góc nhìn
        photonView.RPC("SwitchToSpectatorMode", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    public void SwitchToSpectatorMode(int deadPlayerId)
    {
        if (photonView.IsMine)
        {
            // Tìm một người chơi khác (ngoài chính người chơi chết)
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                SwitchingPOVDieTest pc = player.GetComponent<SwitchingPOVDieTest>();
                if (pc != null && !pc.isDead && pc.photonView.Owner.ActorNumber != deadPlayerId)
                {
                    // Kích hoạt camera của người chơi khác
                    pc.playerCamera.enabled = true;

                    // Hiển thị thông báo spectating
                    if (spectateText != null)
                    {
                        spectateText.gameObject.SetActive(true);
                        spectateText.text = "You are spectating " + pc.photonView.Owner.NickName;
                    }
                    break;
                }
            }
        }
    }
}
