using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;


public class EnemyAi : MonoBehaviour
{
    public Transform[] patrolPoints; // ���� �������� �迭
    public float chaseSpeed = 3.5f; // ���� ���� �ӵ�
    public float patrolSpeed = 2f; // ���� ���� �ӵ�
    public float detectionRange = 10f; // �÷��̾� ���� ����
    public float fieldOfView = 45f; // �þ߰�
    public LayerMask obstacleMask; // ��ֹ� ���̾� ����ũ
    public float lostSightDelay = 2f; // �÷��̾ ������ �� ������ ���߱������ ���� �ð�
    public bool isGuard = false; // Ư�� ������Ʈ�� ��Ű�� ��� ����
    public Transform guardPoint; // Ư�� ������Ʈ�� ��ǥ
    public float guardRangeIncrease = 15f; // �̵� �� ������ ���� ����
    public float guardWaitTime = 4f; // Ư�� ������Ʈ���� ����� �ð�

    private NavMeshAgent myAgent;
    private int currentPatrolIndex = 0; // ���� ���� ���� ������ �ε���
    private Vector3 initialPosition; // �ʱ� ��ġ
    private Transform player; // �÷��̾��� Transform
    private bool isChasingPlayer = false; // �÷��̾ ���� ������ ����
    private float lostSightTimer = 0f; // �÷��̾ ��ģ �� ��� �ð�
    private float originalDetectionRange; // ������ ���� ����
    private bool isGuardActive = false; // ���� ��尡 Ȱ��ȭ�Ǿ����� ����

    public GameObject modeQ;
    public GameObject modeE;

    public GameOver over;

    public bool enemy = false;

    private Collider enemyC;

    void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
        initialPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player").transform; // �±׸� ���� �÷��̾��� Transform�� ������
        originalDetectionRange = detectionRange; // ������ ���� ������ ����
        StartPatrol(); // ���� ����

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
       
        // ���� ������ �������� �� ���� �������� �̵�
        if (myAgent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            myAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }

        // �÷��̾� ���� ����
        DetectPlayer();
    }

    void Guard()
    {
        detectionRange = originalDetectionRange + guardRangeIncrease;
        myAgent.speed = patrolSpeed;

        // ���� ����Ʈ�� z ��ǥ�� �����ϰ� ���ο� ��ġ ����
        Vector3 newGuardPosition = new Vector3(guardPoint.position.x, guardPoint.position.y, guardPoint.position.z +20f);
        myAgent.SetDestination(newGuardPosition);

        // �÷��̾� ���� ����
        DetectPlayer();

        // �÷��̾ �����Ǹ� ���� ���� ��ȯ
        if (isChasingPlayer)
        {
            //return;
            isGuard = false;
        }

        // Ư�� ������Ʈ�� �������� �� ���
        if (myAgent.remainingDistance < 0.5f)
        {
            StartCoroutine(WaitAtGuardPoint());
        }
    }

    IEnumerator WaitAtGuardPoint()
    {
        yield return new WaitForSeconds(guardWaitTime);
        detectionRange = originalDetectionRange; // ���� ������ ������� �ǵ���
        isGuard = false; // ���� ��带 ��Ȱ��ȭ
        isGuardActive = false; // ���� ��尡 Ȱ��ȭ���� �ʵ��� ����
        StartPatrol(); // �ٽ� ���� ���� ���ư�
    }

    void ChasePlayer()
    {
        myAgent.speed = chaseSpeed;
        myAgent.SetDestination(player.position);

        // �÷��̾ ���� �߿��� �÷��̾���� �Ÿ��� ���� �������� ũ�� ������ ����
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

        // �÷��̾ ���� ���� ���� �ִ��� Ȯ��
        if (distanceToPlayer < detectionRange)
        {
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            // �÷��̾ �þ߰� ���� �ִ��� Ȯ��
            if (angleToPlayer < fieldOfView / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, detectionRange, ~obstacleMask))
                {
                    // ����ĳ��Ʈ�� �÷��̾�� �浹�ߴ��� Ȯ��
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

        // ���� ������ �����ϴ� ���� �׸��ϴ�.
        Gizmos.color = Color.cyan;
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            int nextIndex = (i + 1) % patrolPoints.Length;
            Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[nextIndex].position);
        }
    }
}


