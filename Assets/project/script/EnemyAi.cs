using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;


public class EnemyAi : MonoBehaviour
{
    public Transform[] patrolPoints; // 순찰 지점들의 배열
    public float chaseSpeed = 3.5f; // 적의 추적 속도
    public float patrolSpeed = 2f; // 적의 순찰 속도
    public float detectionRange = 10f; // 플레이어 감지 범위
    public float fieldOfView = 45f; // 시야각
    public LayerMask obstacleMask; // 장애물 레이어 마스크
    public float lostSightDelay = 2f; // 플레이어를 놓쳤을 때 추적을 멈추기까지의 지연 시간
    public bool isGuard = false; // 특정 오브젝트를 지키는 모드 여부
    public Transform guardPoint; // 특정 오브젝트의 좌표
    public float guardRangeIncrease = 15f; // 이동 시 증가할 감지 범위
    public float guardWaitTime = 4f; // 특정 오브젝트에서 대기할 시간

    private NavMeshAgent myAgent;
    private int currentPatrolIndex = 0; // 현재 순찰 중인 지점의 인덱스
    private Vector3 initialPosition; // 초기 위치
    private Transform player; // 플레이어의 Transform
    private bool isChasingPlayer = false; // 플레이어를 추적 중인지 여부
    private float lostSightTimer = 0f; // 플레이어를 놓친 후 경과 시간
    private float originalDetectionRange; // 원래의 감지 범위
    private bool isGuardActive = false; // 가드 모드가 활성화되었는지 여부

    public GameObject modeQ;
    public GameObject modeE;

    public GameOver over;

    public bool enemy = false;

    private Collider enemyC;

    void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
        initialPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player").transform; // 태그를 통해 플레이어의 Transform을 가져옴
        originalDetectionRange = detectionRange; // 원래의 감지 범위를 저장
        StartPatrol(); // 순찰 시작

        modeQ.SetActive(false);
        modeE.SetActive(false);

        enemy = false;

        enemyC = GetComponent<Collider>();
    }

    void Update()
    {
        if (isGuard)
        {
            Guard();
            modeQ.SetActive(true);
            modeE.SetActive(false);
        }
        else if (isChasingPlayer)
        {
            ChasePlayer();
            modeQ.SetActive(false);
            modeE.SetActive(true);

        }
        else
        {
            Patrol();
            modeQ.SetActive(false);
            modeE.SetActive(false);

        }
    }

    public void ActivateGuardMode()
    {
        if (!isGuardActive)
        {
            isGuardActive = true;
            isGuard = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("P.");
            over.GetComponent<GameOver>().OverSceneOn();
        }
    }

    void StartPatrol()
    {
        myAgent.speed = patrolSpeed;
        myAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    void Patrol()
    {
       
        // 순찰 지점에 도착했을 때 다음 지점으로 이동
        if (myAgent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            myAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }

        // 플레이어 감지 로직
        DetectPlayer();
    }

    void Guard()
    {
        detectionRange = originalDetectionRange + guardRangeIncrease;
        myAgent.speed = patrolSpeed;

        // 가드 포인트의 z 좌표를 변경하고 새로운 위치 설정
        Vector3 newGuardPosition = new Vector3(guardPoint.position.x, guardPoint.position.y, guardPoint.position.z +20f);
        myAgent.SetDestination(newGuardPosition);

        // 플레이어 감지 로직
        DetectPlayer();

        // 플레이어가 감지되면 추적 모드로 전환
        if (isChasingPlayer)
        {
            //return;
            isGuard = false;
        }

        // 특정 오브젝트에 도착했을 때 대기
        if (myAgent.remainingDistance < 0.5f)
        {
            StartCoroutine(WaitAtGuardPoint());
        }
    }

    IEnumerator WaitAtGuardPoint()
    {
        yield return new WaitForSeconds(guardWaitTime);
        detectionRange = originalDetectionRange; // 감지 범위를 원래대로 되돌림
        isGuard = false; // 가드 모드를 비활성화
        isGuardActive = false; // 가드 모드가 활성화되지 않도록 설정
        StartPatrol(); // 다시 순찰 모드로 돌아감
    }

    void ChasePlayer()
    {
        myAgent.speed = chaseSpeed;
        myAgent.SetDestination(player.position);

        // 플레이어를 추적 중에는 플레이어와의 거리가 감지 범위보다 크면 추적을 멈춤
        if (Vector3.Distance(transform.position, player.position) > detectionRange)
        {

            lostSightTimer += Time.deltaTime;
            if (lostSightTimer >= lostSightDelay)
            {
                StopChasingPlayer();
            }
        }
        else
        {
            lostSightTimer = 0f;
        }

        
    }

    void DetectPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // 플레이어가 감지 범위 내에 있는지 확인
        if (distanceToPlayer < detectionRange)
        {
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            // 플레이어가 시야각 내에 있는지 확인
            if (angleToPlayer < fieldOfView / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, detectionRange, ~obstacleMask))
                {
                    // 레이캐스트가 플레이어와 충돌했는지 확인
                    if (hit.transform == player)
                    {
                        //Debug.Log("Player detected!");
                        isChasingPlayer = true;
                    }
                    else
                    {
                        //Debug.Log("Player not in sight, obstacle in the way.");
                    }
                }
            }
        }
    }

    void StopChasingPlayer()
    {
        modeQ.SetActive(false);
        modeE.SetActive(false);

        Debug.Log("Lost sight of player. Returning to patrol.");
        isChasingPlayer = false;
        myAgent.speed = patrolSpeed;
        myAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Vector3 fovLine1 = Quaternion.AngleAxis(fieldOfView / 2, Vector3.up) * transform.forward * detectionRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-fieldOfView / 2, Vector3.up) * transform.forward * detectionRange;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        if (player != null)
        {
            Vector3 directionToPlayer = player.position - transform.position;
            if (Vector3.Angle(transform.forward, directionToPlayer) < fieldOfView / 2)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, directionToPlayer.normalized * detectionRange);
            }
        }

        // 순찰 지점을 연결하는 선을 그립니다.
        Gizmos.color = Color.cyan;
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            int nextIndex = (i + 1) % patrolPoints.Length;
            Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[nextIndex].position);
        }
    }
}


