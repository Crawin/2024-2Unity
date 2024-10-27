using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobNav : MonoBehaviour
{
    NavMeshAgent agent;
    public float range = 10.0f;  // �̵��� ��ġ�� �ִ� �ݰ�
    float escapeRange;
    private float stuckTimer;
    public float stuckCheckInterval = 2.0f;  // ���� ���� ����
    private Vector3 lastPosition;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        escapeRange = range*2;
        //SetRandomDestination();
    }

    // Update is called once per frame
    void Update()
    {
        // �������� ���� �����ߴ��� Ȯ��
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                SetRandomDestination();  // ���ο� ������ ����
            }
        }
        stuckTimer += Time.deltaTime;
        if (stuckTimer >= stuckCheckInterval)
        {
            if (Vector3.Distance(transform.position, lastPosition) < 0.5f)  // ���� �������� ������
            {
                FindEscapeDestination();  // �� ���� �������� Ż�� ��ġ ã��
            }
            lastPosition = transform.position;
            stuckTimer = 0;
        }
    }
    void FindEscapeDestination()
    {
        float xRandom = Random.Range(-escapeRange, escapeRange);  // X�࿡�� ���� �� ����
        float zRandom = Random.Range(-escapeRange, escapeRange);  // Z�࿡�� ���� �� ����
        Vector3 escapeDirection = new Vector3(xRandom, 0f, zRandom) + transform.position;  // Y�� 0���� ����

        NavMeshHit hit;
        if (NavMesh.SamplePosition(escapeDirection, out hit, escapeRange, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);  // �� ���� �������� ��ǥ ����
        }
        else
        {
            SetRandomDestination();  // ���� ���� ������ ���ο� ��ǥ ����
        }
    }
    void SetRandomDestination()
    {
        float xRandom = Random.Range(-range, range);  // X�࿡�� ���� �� ����
        float zRandom = Random.Range(-range, range);  // Z�࿡�� ���� �� ����
        Vector3 randomDirection = new Vector3(xRandom, 0f, zRandom) + transform.position;  // Y�� 0���� ����

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, range, NavMesh.AllAreas))
        {
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(hit.position, path);
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                agent.SetDestination(hit.position);
            }
            else
            {
                FindEscapeDestination();  // ��ȿ�� ��ΰ� �ƴ϶�� Ż�� ��ġ�� ����
            }
        }
    }
}
