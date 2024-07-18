using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detect : MonoBehaviour
{
    public EnemyAi enemyPatrol; // 적 캐릭터의 EnemyPatrol 스크립트
    public float cooldownTime = 10f; // 쿨타임 시간
    private bool isCooldown = false; // 쿨타임 여부

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCooldown)
        {
            enemyPatrol.ActivateGuardMode();
            StartCoroutine(CooldownCoroutine());
        }
    }

    private IEnumerator CooldownCoroutine()
    {
        isCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isCooldown = false;
    }
}

