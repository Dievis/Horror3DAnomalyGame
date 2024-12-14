using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class EnemyAI : MonoBehaviourPunCallbacks
{
    public NavMeshAgent ai;
    public List<Transform> destinations;
    public Animator aiAnim;
    public float walkSpeed, chaseSpeed, minIdleTime, maxIdleTime, detectionDistance, catchDistance, searchDistance, minChaseTime, maxChaseTime, minSearchTime, maxSearchTime, jumpscareTime;
    public float idleTime = 0f; // Đảm bảo giá trị khởi tạo
    public bool walking, chasing, searching;
    public Transform player;
    Transform currentDest;
    Vector3 dest;
    public Vector3 rayCastOffset;
    public GameObject hideText, stopHideText;
    public GameObject deathPanel;

    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();

        // Chỉ Master Client điều khiển quái vật
        if (PhotonNetwork.IsMasterClient)
        {
            deathPanel.SetActive(false);
            walking = true;
            currentDest = destinations[Random.Range(0, destinations.Count)];
        }
    }

    void Update()
    {
        // Chỉ Master Client mới cập nhật trạng thái quái vật
        if (!PhotonNetwork.IsMasterClient) return;

        Vector3 direction = (player.position - transform.position).normalized;
        RaycastHit hit;
        float aiDistance = Vector3.Distance(player.position, this.transform.position);

        if (Physics.Raycast(transform.position + rayCastOffset, direction, out hit, detectionDistance))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                walking = false;
                StopAllCoroutines();
                StartCoroutine("searchRoutine");
                searching = true;
            }
        }

        if (searching)
        {
            ai.speed = 0;
            SyncAnimation("search");
            if (aiDistance <= searchDistance)
            {
                StopAllCoroutines();
                StartCoroutine("chaseRoutine");
                chasing = true;
                searching = false;
            }
        }

        if (chasing)
        {
            dest = player.position;
            ai.destination = dest;
            ai.speed = chaseSpeed;
            SyncAnimation("sprint");

            if (aiDistance <= catchDistance)
            {
                photonView.RPC("PlayerCaught", RpcTarget.All);
                chasing = false;
            }
        }

        if (walking)
        {
            dest = currentDest.position;
            ai.destination = dest;
            ai.speed = walkSpeed;
            SyncAnimation("walk");

            if (ai.remainingDistance <= ai.stoppingDistance)
            {
                SyncAnimation("idle");
                ai.speed = 0;
                StopAllCoroutines();
                StartCoroutine("stayIdle");
                walking = false;
            }
        }
    }

    [PunRPC]
    void PlayerCaught()
    {
        deathPanel.SetActive(true); // Hiển thị deathPanel
        player.gameObject.SetActive(false); // Tắt player
        SyncAnimation("jumpscare");

        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(deathRoutine());
    }

    void SyncAnimation(string trigger)
    {
        aiAnim.ResetTrigger("walk");
        aiAnim.ResetTrigger("idle");
        aiAnim.ResetTrigger("search");
        aiAnim.ResetTrigger("sprint");
        aiAnim.ResetTrigger("jumpscare");
        aiAnim.SetTrigger(trigger);
    }

    public void stopChase()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        walking = true;
        chasing = false;
        StopAllCoroutines();
        currentDest = destinations[Random.Range(0, destinations.Count)];
    }

    IEnumerator stayIdle()
    {
        idleTime = Random.Range(minIdleTime, maxIdleTime);
        yield return new WaitForSeconds(idleTime);
        walking = true;
        currentDest = destinations[Random.Range(0, destinations.Count)];
    }

    IEnumerator searchRoutine()
    {
        yield return new WaitForSeconds(Random.Range(minSearchTime, maxSearchTime));
        searching = false;
        walking = true;
        currentDest = destinations[Random.Range(0, destinations.Count)];
    }

    IEnumerator chaseRoutine()
    {
        yield return new WaitForSeconds(Random.Range(minChaseTime, maxChaseTime));
        stopChase();
    }

    IEnumerator deathRoutine()
    {
        yield return new WaitForSeconds(jumpscareTime);
        PhotonNetwork.LoadLevel("MainMenuScene"); // Chuyển cảnh khi chết
    }
}
