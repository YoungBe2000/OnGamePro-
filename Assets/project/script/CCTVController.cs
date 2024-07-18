using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(LineRenderer))]
public class CCTVController : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public float detectionRange = 10f; // 감지 범위
    public float fieldOfView = 45f; // 시야각
    public LayerMask detectionLayer; // 장애물 레이어 마스크
    public float rotationSpeed = 30f; // CCTV 회전 속도
    public float rotationAngle = 45f; // CCTV가 회전할 각도
    public float rotationPauseTime = 2f; // 각도 사이에서 멈출 시간
    public Color detectionRangeColor = Color.red; // 감지 범위 색상
    public int resolution = 50; // 시야각도를 그릴 때의 해상도
    public LayerMask groundLayer; // 바닥 레이어 마스크

    private bool isRotating = true;
    private float currentRotationAngle = 0f;
    private float rotationDirection = 1f;
    private float rotationPauseTimer = 0f;
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = resolution + 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = detectionRangeColor;
        lineRenderer.endColor = detectionRangeColor;
    }

    void Update()
    {
        RotateCCTV();
        //DetectPlayer();
    }

    void RotateCCTV()
    {
        if (isRotating)
        {
            float rotationStep = rotationSpeed * Time.deltaTime * rotationDirection;
            currentRotationAngle += rotationStep;

            if (Mathf.Abs(currentRotationAngle) >= rotationAngle)
            {
                rotationDirection *= -1;
                isRotating = false;
                rotationPauseTimer = rotationPauseTime;
            }

            transform.Rotate(Vector3.up, rotationStep);
        }
        else
        {
            rotationPauseTimer -= Time.deltaTime;
            if (rotationPauseTimer <= 0f)
            {
                isRotating = true;
            }
        }
    }

    //void DetectPlayer()
    //{
    //    Vector3 directionToPlayer = player.position - transform.position;
    //    float distanceToPlayer = directionToPlayer.magnitude;

    //    // 플레이어가 감지 범위 내에 있는지 확인
    //    if (distanceToPlayer < detectionRange)
    //    {
    //        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

    //        // 플레이어가 시야각 내에 있는지 확인
    //        if (angleToPlayer < fieldOfView / 2)
    //        {
    //            RaycastHit hit;
    //            if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, detectionRange, ~detectionLayer))
    //            {
    //                // 레이캐스트가 플레이어와 충돌했는지 확인
    //                if (hit.transform == player)
    //                {
    //                    //Debug.Log("Player detected!");
    //                }
    //            }
    //        }
    //    }
    //}



    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Vector3 fovLine1 = Quaternion.AngleAxis(fieldOfView / 2, Vector3.up) * transform.forward * detectionRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-fieldOfView / 2, Vector3.up) * transform.forward * detectionRange;

        //Gizmos.color = Color.blue;
        //Gizmos.DrawRay(transform.position, fovLine1);
        //Gizmos.DrawRay(transform.position, fovLine2);

        if (player != null)
        {
            Vector3 directionToPlayer = player.position - transform.position;
            if (Vector3.Angle(transform.forward, directionToPlayer) < fieldOfView / 2)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, directionToPlayer.normalized * detectionRange);
            }
        }
    }
}







