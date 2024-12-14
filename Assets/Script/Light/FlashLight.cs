using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    public GameObject flashlight;
    public GameObject Flight;
    public AudioSource audioSource; // Add this
    public AudioClip sound1; // Add this
    public AudioClip sound2; // Add this
    private bool PlayerInZone;
    public bool onhand;
    private bool isLightOn = false; // Add this
    void Start()
    {
        flashlight.SetActive(true);
        Flight.SetActive(true);
        PlayerInZone = false;
        onhand = false;
    }


    void Update()
    {
        if (!onhand)
            onhand = flashlight.GetComponent<OnHand>().onHand;
        if (PlayerInZone && Input.GetKeyDown(KeyCode.F) && onhand)           //if in zone and press E key
        {
            Flight.SetActive(!Flight.activeSelf);
            isLightOn = !isLightOn; // Toggle the light state

            // Play the appropriate sound
            if (isLightOn)
            {
                audioSource.clip = sound1;
            }
            else
            {
                audioSource.clip = sound2;
            }
            audioSource.Play();
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")     //if player in zone
        {
            PlayerInZone = true;
        }
    }
}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Photon.Pun;

//public class FlashLight : MonoBehaviourPunCallbacks
//{
//    public GameObject flashlight;   // Đối tượng đèn pin
//    public GameObject Flight;       // Đối tượng ánh sáng của đèn pin
//    public AudioSource audioSource; // Nguồn phát âm thanh
//    public AudioClip sound1;        // Âm thanh khi bật đèn
//    public AudioClip sound2;        // Âm thanh khi tắt đèn

//    private bool PlayerInZone;      // Kiểm tra người chơi trong vùng
//    public bool onhand;             // Kiểm tra đèn có trên tay hay không
//    private bool isLightOn = false; // Trạng thái đèn (bật/tắt)

//    void Start()
//    {
//        flashlight.SetActive(true);
//        Flight.SetActive(false);   // Tắt đèn khi bắt đầu
//        PlayerInZone = false;
//        onhand = false;
//    }

//    void Update()
//    {
//        if (!onhand)
//            onhand = flashlight.GetComponent<OnHand>().onHand;

//        // Kiểm tra nếu người chơi trong vùng và nhấn phím F
//        if (PlayerInZone && Input.GetKeyDown(KeyCode.F) && onhand)
//        {
//            photonView.RPC("RPC_ToggleFlashlight", RpcTarget.All); // Gọi RPC để đồng bộ
//        }
//    }

//    [PunRPC]
//    void RPC_ToggleFlashlight()
//    {
//        // Bật/tắt ánh sáng
//        Flight.SetActive(!Flight.activeSelf);
//        isLightOn = !isLightOn; // Cập nhật trạng thái đèn

//        // Phát âm thanh tương ứng
//        if (isLightOn)
//        {
//            audioSource.clip = sound1;
//        }
//        else
//        {
//            audioSource.clip = sound2;
//        }
//        audioSource.Play();
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.gameObject.tag == "Reach") // Kiểm tra nếu người chơi trong vùng
//        {
//            PlayerInZone = true;
//        }
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (other.gameObject.tag == "Reach") // Kiểm tra nếu người chơi rời khỏi vùng
//        {
//            PlayerInZone = false;
//        }
//    }
//}

