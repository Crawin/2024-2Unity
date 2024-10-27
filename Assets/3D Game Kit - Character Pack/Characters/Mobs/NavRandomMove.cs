using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobNav : MonoBehaviour
{
    NavMeshAgent agent;
    public float range = 10.0f;  // 이동할 위치의 최대 반경
    float escapeRange;
    private float stuckTimer;
    public float stuckCheckInterval = 2.0f;  // 갇힘 감지 간격
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
        // 목적지에 거의 도착했는지 확인
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                SetRandomDestination();  // 새로운 목적지 설정
            }
        }
        stuckTimer += Time.deltaTime;
        if (stuckTimer >= stuckCheckInterval)
        {
            if (Vector3.Distance(transform.position, lastPosition) < 0.5f)  // 거의 움직이지 않으면
            {
                FindEscapeDestination();  // 더 넓은 범위에서 탈출 위치 찾기
            }
            lastPosition = transform.position;
            stuckTimer = 0;
        }
    }
    void FindEscapeDestination()
    {
        float xRandom = Random.Range(-escapeRange, escapeRange);  // X축에서 랜덤 값 생성
        float zRandom = Random.Range(-escapeRange, escapeRange);  // Z축에서 랜덤 값 생성
        Vector3 escapeDirection = new Vector3(xRandom, 0f, zRandom) + transform.position;  // Y는 0으로 고정

        NavMeshHit hit;
        if (NavMesh.SamplePosition(escapeDirection, out hit, escapeRange, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);  // 더 넓은 범위에서 목표 설정
        }
        else
        {
            SetRandomDestination();  // 기존 범위 내에서 새로운 목표 설정
        }
    }
    void SetRandomDestination()
    {
        float xRandom = Random.Range(-range, range);  // X축에서 랜덤 값 생성
        float zRandom = Random.Range(-range, range);  // Z축에서 랜덤 값 생성
        Vector3 randomDirection = new Vector3(xRandom, 0f, zRandom) + transform.position;  // Y는 0으로 고정

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
                FindEscapeDestination();  // 유효한 경로가 아니라면 탈출 위치로 설정
            }
        }
    }
}
