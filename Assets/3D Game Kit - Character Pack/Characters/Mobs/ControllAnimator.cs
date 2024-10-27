using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class ControllAnimator : MonoBehaviour
{
    public float MAXHP;
    private float currHP;
    private NavMeshAgent agent;
    private Animator animator;
    private bool Attacking;
    private bool AttackStart;
    public float AttackRange;
    public float DetectRange;
    public float AttackDamage;
    bool isChasing;
    bool isDying;
    public bool IsBoss;
    private GameObject Gameover;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        currHP = MAXHP;
        Gameover = GameObject.Find("GameOver");
    }

    // Update is called once per frame
    void Update()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        if (isDying == false)
        {
            Vector3 Player = DetectPlayer();
            if (isChasing)
            {
                agent.SetDestination(Player);

                // 공격 거리 내에 도달하면 공격 시도
                float distanceToPlayer = Vector3.Distance(transform.position, Player);
                if (distanceToPlayer <= AttackRange && AttackStart == false)
                {
                    AttackStart = true;
                    animator.SetTrigger("Attack");
                    agent.isStopped = true;
                }
            }

            if (Attacking)
            {
                LayerMask layermask = LayerMask.GetMask("Player");
                Vector3 boxCenter = transform.position + transform.forward * AttackRange / 2;
                Vector3 boxHalfExtents = new Vector3(1, 1, AttackRange / 2);
                Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxHalfExtents, transform.rotation, layermask);
                foreach (var hitCollider in hitColliders)
                {
                    Attacking = false;
                    //Debug.Log("전방 범위 내의 오브젝트: " + hitCollider.gameObject.name);
                    hitCollider.GetComponent<AllenInput>().Damage(AttackDamage);
                }
            }
        }
    }
    Vector3 DetectPlayer()
    {
        Collider[] InRange = Physics.OverlapSphere(transform.position, DetectRange,LayerMask.GetMask("Player"));
        foreach (var hitCollider in InRange)
        {
            // NPC와 플레이어 사이의 방향 벡터 및 거리 계산
            Vector3 directionToPlayer = (hitCollider.transform.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, hitCollider.transform.position);

            // 플레이어가 시야 거리와 각도 내에 있는지 확인
            if (Vector3.Angle(transform.forward, directionToPlayer) < 45 / 2)
            {
                // Raycast를 사용하여 NPC와 플레이어 사이에 장애물이 있는지 확인
                if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, LayerMask.GetMask("Default")))
                {
                    // 플레이어가 NPC의 시야 내에 있음
                    isChasing = true;
                    return hitCollider.transform.position;
                }
            }
            else
            {
                // 플레이어가 시야를 벗어나면 추적 상태 해제
                if (isChasing)
                {
                    isChasing = false;
                    agent.ResetPath();
                }
            }
        }
        return new Vector3(0,0,0);
    }
    public void GetDamage(float damage)
    {
        if (isDying == false)
        {
            if (IsBoss)
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                int LeftEnemies = enemies.Length;
                float damagePercent = LeftEnemies * 0.1f;
                //Debug.Log(LeftEnemies);
                if (damagePercent > 1) {
                    damagePercent = 1; 
                }
                damagePercent = 1 - damagePercent;
                damage *= damagePercent;
            }
            //Debug.Log(gameObject.name + "이 " + damage + "피해를 입음");
            agent.isStopped = true;
            currHP -= damage;
            if (currHP <= 0)
            {
                isDying = true;
                animator.SetTrigger("Die");
            }
            else
            {
                animator.SetTrigger("Hurt");
            }
        }
    }
    void Dead()
    {
        Destroy(gameObject);
        if (IsBoss)
        {
            Gameover.GetComponent<GameOver>().Complete();
        }
    }

    void HurtEnd()
    {
        //Debug.Log("hurt end");
        agent.isStopped = false;
    }

    void PlayStep()
    {
    }

    void AttackBegin()
    {
        Attacking = true;
    }
    void AttackEnd()
    {
        AttackStart = false;
        Attacking = false;
        agent.isStopped = false;
    }

    void Grunt()
    {
    }
}
