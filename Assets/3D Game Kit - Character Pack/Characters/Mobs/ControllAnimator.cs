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

                // ���� �Ÿ� ���� �����ϸ� ���� �õ�
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
                    //Debug.Log("���� ���� ���� ������Ʈ: " + hitCollider.gameObject.name);
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
            // NPC�� �÷��̾� ������ ���� ���� �� �Ÿ� ���
            Vector3 directionToPlayer = (hitCollider.transform.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, hitCollider.transform.position);

            // �÷��̾ �þ� �Ÿ��� ���� ���� �ִ��� Ȯ��
            if (Vector3.Angle(transform.forward, directionToPlayer) < 45 / 2)
            {
                // Raycast�� ����Ͽ� NPC�� �÷��̾� ���̿� ��ֹ��� �ִ��� Ȯ��
                if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, LayerMask.GetMask("Default")))
                {
                    // �÷��̾ NPC�� �þ� ���� ����
                    isChasing = true;
                    return hitCollider.transform.position;
                }
            }
            else
            {
                // �÷��̾ �þ߸� ����� ���� ���� ����
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
            //Debug.Log(gameObject.name + "�� " + damage + "���ظ� ����");
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
