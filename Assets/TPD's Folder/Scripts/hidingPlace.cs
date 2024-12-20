using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hidingPlace : MonoBehaviour
{
    public GameObject hideText, stopHideText;
    public GameObject normalPlayer, hidingPlayer;

    // Danh sách chứa tất cả các quái có EnemyAI
    public List<EnemyAI> monsterScripts = new List<EnemyAI>();
    public List<Transform> monsterTransforms = new List<Transform>();

    public float loseDistance;

    private bool interactable = false;
    private bool hiding = false;
    private bool beingAttacked = false; // Biến để kiểm tra bị tấn công
    public Rigidbody rb;
    public GameObject playerCamera, moDel;
    public CapsuleCollider playerCapsule;

    void Start()
    {
        rb = normalPlayer.GetComponent<Rigidbody>();
        playerCapsule = normalPlayer.GetComponent<CapsuleCollider>();

        // Tự động tìm tất cả các quái có EnemyAI và thêm vào danh sách
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>(); // Tìm tất cả các script EnemyAI
        foreach (EnemyAI enemy in enemies)
        {
            monsterScripts.Add(enemy);
            monsterTransforms.Add(enemy.transform); // Lưu cả transform của quái
        }
    }

    void OnTriggerStay(Collider other)
    {
        // Nếu quái không tấn công và "Reach" chạm vào collider
        if (other.CompareTag("Reach") && !hiding && !beingAttacked)
        {
            hideText.SetActive(true);
            interactable = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Nếu "Reach" rời khỏi collider, ẩn HideText
        if (other.CompareTag("Reach"))
        {
            hideText.SetActive(false);
            interactable = false;
        }
    }

    void Update()
    {
        // Kiểm tra nếu bất kỳ quái nào đang chasing và người chơi đang trốn
        foreach (EnemyAI monster in monsterScripts)
        {
            if (monster.chasing && hiding)
            {
                beingAttacked = true;
                hideText.SetActive(false);
                stopHideText.SetActive(false);
                break;
            }
        }

        // Kiểm tra ấn phím E khi tương tác
        if (interactable && Input.GetKeyDown(KeyCode.E) && !hiding)
        {
            StartHiding();
        }
        else if (hiding && Input.GetKeyDown(KeyCode.E))
        {
            StopHiding();
        }
    }

    void StartHiding()
    {
        hideText.SetActive(false);
        stopHideText.SetActive(true);
        hidingPlayer.SetActive(true);
        hiding = true;

        rb.useGravity = false;
        playerCamera.SetActive(false);
        moDel.SetActive(false);
        playerCapsule.enabled = false;

        // Kiểm tra khoảng cách và dừng chase cho các quái nếu cần
        for (int i = 0; i < monsterScripts.Count; i++)
        {
            float distance = Vector3.Distance(monsterTransforms[i].position, normalPlayer.transform.position);
            if (distance > loseDistance && monsterScripts[i].chasing)
            {
                monsterScripts[i].stopChase();
            }
        }
    }

    void StopHiding()
    {
        if (!beingAttacked) // Chỉ thoát ra nếu không bị quái tấn công
        {
            stopHideText.SetActive(false);
            hidingPlayer.SetActive(false);
            hiding = false;

            rb.useGravity = true;
            playerCamera.SetActive(true);
            moDel.SetActive(true);
            playerCapsule.enabled = true;
        }
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class hidingPlace : MonoBehaviour
//{
//    public GameObject hideText, stopHideText;
//    public GameObject normalPlayer, hidingPlayer;

//    // Danh sách chứa tất cả các quái có EnemyAI
//    public List<EnemyAI> monsterScripts = new List<EnemyAI>();
//    public List<Transform> monsterTransforms = new List<Transform>();

//    public float loseDistance;
//    public float raycastLength = 3f; // Chiều dài raycast

//    private bool interactable = false;
//    private bool hiding = false;
//    private bool beingAttacked = false; // Biến để kiểm tra bị tấn công
//    public Rigidbody rb;
//    public GameObject playerCamera, moDel;
//    public CapsuleCollider playerCapsule;

//    private Camera playerCam;

//    void Start()
//    {
//        rb = normalPlayer.GetComponent<Rigidbody>();
//        playerCapsule = normalPlayer.GetComponent<CapsuleCollider>();

//        playerCam = normalPlayer.GetComponentInChildren<Camera>(); // Camera của player

//        // Tự động tìm tất cả các quái có EnemyAI và thêm vào danh sách
//        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>(); // Tìm tất cả các script EnemyAI
//        foreach (EnemyAI enemy in enemies)
//        {
//            monsterScripts.Add(enemy);
//            monsterTransforms.Add(enemy.transform); // Lưu cả transform của quái
//        }
//    }

//    void Update()
//    {
//        // Kiểm tra raycast tương tác với các đối tượng trốn
//        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
//        RaycastHit hit;

//        if (Physics.Raycast(ray, out hit, raycastLength))
//        {
//            if (hit.collider.CompareTag("HidingPlace") && !hiding && !beingAttacked)
//            {
//                hideText.SetActive(true);
//                interactable = true;
//            }
//            else
//            {
//                hideText.SetActive(false);
//                interactable = false;
//            }

//            // Nếu người chơi nhấn E và đang đứng gần nơi có thể trốn
//            if (interactable && Input.GetKeyDown(KeyCode.E) && !hiding)
//            {
//                StartHiding();
//            }
//            else if (hiding && Input.GetKeyDown(KeyCode.E))
//            {
//                StopHiding();
//            }
//        }
//        else
//        {
//            hideText.SetActive(false);
//            interactable = false;
//        }

//        // Kiểm tra nếu bất kỳ quái nào đang chasing và người chơi đang trốn
//        foreach (EnemyAI monster in monsterScripts)
//        {
//            if (monster.chasing && hiding)
//            {
//                beingAttacked = true;
//                hideText.SetActive(false);
//                stopHideText.SetActive(false);
//                break;
//            }
//        }
//    }

//    void StartHiding()
//    {
//        hideText.SetActive(false);
//        stopHideText.SetActive(true);
//        hidingPlayer.SetActive(true);
//        hiding = true;

//        rb.useGravity = false;
//        playerCamera.SetActive(false);
//        moDel.SetActive(false);
//        playerCapsule.enabled = false;

//        // Kiểm tra khoảng cách và dừng chase cho các quái nếu cần
//        for (int i = 0; i < monsterScripts.Count; i++)
//        {
//            float distance = Vector3.Distance(monsterTransforms[i].position, normalPlayer.transform.position);
//            if (distance > loseDistance && monsterScripts[i].chasing)
//            {
//                monsterScripts[i].stopChase();
//            }
//        }
//    }

//    void StopHiding()
//    {
//        if (!beingAttacked) // Chỉ thoát ra nếu không bị quái tấn công
//        {
//            stopHideText.SetActive(false);
//            hidingPlayer.SetActive(false);
//            hiding = false;

//            rb.useGravity = true;
//            playerCamera.SetActive(true);
//            moDel.SetActive(true);
//            playerCapsule.enabled = true;
//        }
//    }
//}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class hidingPlace : MonoBehaviour
//{
//    public GameObject hideText, stopHideText;
//    public GameObject normalPlayer;
//    public Camera hidingCamera; // Camera sử dụng khi trốn

//    public List<EnemyAI> monsterScripts = new List<EnemyAI>();
//    public List<Transform> monsterTransforms = new List<Transform>();

//    public float loseDistance;

//    private bool interactable = false;
//    private bool hiding = false;
//    private bool beingAttacked = false; // Biến để kiểm tra bị tấn công
//    public Rigidbody rb;
//    public GameObject playerCamera, moDel;
//    public CapsuleCollider playerCapsule;

//    void Start()
//    {
//        rb = normalPlayer.GetComponent<Rigidbody>();
//        playerCapsule = normalPlayer.GetComponent<CapsuleCollider>();

//        // Tự động tìm tất cả các quái có EnemyAI và thêm vào danh sách
//        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
//        foreach (EnemyAI enemy in enemies)
//        {
//            monsterScripts.Add(enemy);
//            monsterTransforms.Add(enemy.transform);
//        }
//    }

//    void OnTriggerStay(Collider other)
//    {
//        if (other.CompareTag("Reach") && !hiding && !beingAttacked)
//        {
//            hideText.SetActive(true);
//            interactable = true;
//        }
//    }

//    void OnTriggerExit(Collider other)
//    {
//        if (other.CompareTag("Reach"))
//        {
//            hideText.SetActive(false);
//            interactable = false;
//        }
//    }

//    void Update()
//    {
//        foreach (EnemyAI monster in monsterScripts)
//        {
//            if (monster.chasing && hiding)
//            {
//                beingAttacked = true;
//                hideText.SetActive(false);
//                stopHideText.SetActive(false);
//                break;
//            }
//        }

//        if (interactable && Input.GetKeyDown(KeyCode.E) && !hiding)
//        {
//            StartHiding();
//        }
//        else if (hiding && Input.GetKeyDown(KeyCode.E))
//        {
//            StopHiding();
//        }
//    }

//    void StartHiding()
//    {
//        hideText.SetActive(false);
//        stopHideText.SetActive(true);
//        hiding = true;

//        // Tắt các thành phần của player
//        //rb.useGravity = false;
//        playerCamera.SetActive(false);
//        moDel.SetActive(false);
//        playerCapsule.enabled = false;

//        // Bật camera trốn
//        hidingCamera.gameObject.SetActive(true);

//        // Kiểm tra khoảng cách và dừng chase cho các quái nếu cần
//        for (int i = 0; i < monsterScripts.Count; i++)
//        {
//            float distance = Vector3.Distance(monsterTransforms[i].position, transform.position);
//            if (distance > loseDistance && monsterScripts[i].chasing)
//            {
//                monsterScripts[i].stopChase();
//            }
//        }
//    }

//    void StopHiding()
//    {
//        if (!beingAttacked)
//        {
//            stopHideText.SetActive(false);
//            hiding = false;

//            // Bật lại các thành phần của player
//            //rb.useGravity = true;
//            playerCamera.SetActive(true);
//            moDel.SetActive(true);
//            playerCapsule.enabled = true;

//            // Tắt camera trốn
//            hidingCamera.gameObject.SetActive(false);
//        }
//    }
//}

