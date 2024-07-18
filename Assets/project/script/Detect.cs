using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detect : MonoBehaviour
{
    public EnemyAi enemyPatrol; // �� ĳ������ EnemyPatrol ��ũ��Ʈ
    public float cooldownTime = 10f; // ��Ÿ�� �ð�
    private bool isCooldown = false; // ��Ÿ�� ����

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

