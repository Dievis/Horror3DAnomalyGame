using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent ai;
    public List<Transform> destinations;
    public Animator aiAnim;
    public float walkSpeed, chaseSpeed, minIdleTime, maxIdleTime, idleTime, detectionDistance, catchDistance, minChaseTime, maxChaseTime, jumpscareTime;
    public bool walking, chasing;
    public Transform player;
    Transform currentDest;
    Vector3 dest;
    public Vector3 rayCastOffset;
    public float aiDistance;
    public GameObject hideText, stopHideText;
    public GameObject deathPanel;
    public GameObject cameRa;
    public AudioSource deathAudio; // AudioSource để phát âm thanh
    public AudioSource walkingAudio;
    public AudioSource chasingAudio;


    void Start()
    {
        walking = true;
        currentDest = destinations[Random.Range(0, destinations.Count)];
    }
    void Update()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        RaycastHit hit;
        aiDistance = Vector3.Distance(player.position, this.transform.position);
        if (Physics.Raycast(transform.position + rayCastOffset, direction, out hit, detectionDistance))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                walking = false;
                StopCoroutine("stayIdle");
                StopCoroutine("chaseRoutine");
                StartCoroutine("chaseRoutine");
                chasing = true;
            }
        }
        if (chasing == true)
        {
            dest = player.position;
            ai.destination = dest;
            ai.speed = chaseSpeed;
            aiAnim.ResetTrigger("walk");
            aiAnim.ResetTrigger("idle");
            aiAnim.SetTrigger("sprint");

            // Dừng âm thanh khi đang chasing
            if (walkingAudio.isPlaying)
            {
                walkingAudio.Stop();
            }

            // Phát âm thanh chasing
            if (!chasingAudio.isPlaying)
            {
                chasingAudio.Play();
            }


            float distance = Vector3.Distance(player.position, ai.transform.position);
            if (distance <= catchDistance)
            {
                player.gameObject.SetActive(false);
                aiAnim.ResetTrigger("walk");
                aiAnim.ResetTrigger("idle");
                hideText.SetActive(false);
                stopHideText.SetActive(false);
                aiAnim.ResetTrigger("sprint");
                aiAnim.SetTrigger("jumpscare");
                StartCoroutine(deathRoutine());
                chasing = false;

                // Dừng âm thanh chasing khi bắt được người chơi
                if (chasingAudio.isPlaying)
                {
                    chasingAudio.Stop();
                }
            }
        }
        if (walking == true)
        {
            dest = currentDest.position;
            ai.destination = dest;
            ai.speed = walkSpeed;
            aiAnim.ResetTrigger("sprint");
            aiAnim.ResetTrigger("idle");
            aiAnim.SetTrigger("walk");

            // Phát âm thanh khi đang walking
            if (!walkingAudio.isPlaying)
            {
                walkingAudio.Play();
            }

            // Dừng âm thanh chasing nếu Enemy không chasing nữa
            if (chasingAudio.isPlaying)
            {
                chasingAudio.Stop();
            }

            if (ai.remainingDistance <= ai.stoppingDistance)
            {
                aiAnim.ResetTrigger("sprint");
                aiAnim.ResetTrigger("walk");
                aiAnim.SetTrigger("idle");

                // Dừng âm thanh nếu không phải trạng thái walking
                if (walkingAudio.isPlaying)
                {
                    walkingAudio.Stop();
                }



                ai.speed = 0;
                StopCoroutine("stayIdle");
                StartCoroutine("stayIdle");
                walking = false;
            }
        }
        else
        {
            // Dừng âm thanh nếu không phải trạng thái walking
            if (walkingAudio.isPlaying)
            {
                walkingAudio.Stop();
            }
        }
    }
    public void stopChase()
    {
        walking = true;
        chasing = false;

        if (chasingAudio.isPlaying)
        {
            chasingAudio.Stop();
        }

        StopCoroutine("chaseRoutine");
        currentDest = destinations[Random.Range(0, destinations.Count)];


    }
    IEnumerator stayIdle()
    {
        idleTime = Random.Range(minIdleTime, maxIdleTime);
        yield return new WaitForSeconds(idleTime);
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
        cameRa.SetActive(true);

        // Phát âm thanh khi chết
        if (deathAudio != null)
        {
            deathAudio.Play();
        }

        yield return new WaitForSeconds(jumpscareTime);
        deathPanel.SetActive(true);
        UnlockCursor();
    }
    private void UnlockCursor()
    {
        // Mở khóa con trỏ khi cần thiết
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}