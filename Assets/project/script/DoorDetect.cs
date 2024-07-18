using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DoorDetect : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // �÷��̾�� ����� �� �ֵ��� �ƹ� �͵� ���� ����
        }
        else if (other.CompareTag("Enemy"))
        {
            // ���� ������� ���ϵ��� ����
            other.GetComponent<Collider>().isTrigger = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // ���� ������ ����� �ٽ� Trigger�� ����
            other.GetComponent<Collider>().isTrigger = true;
        }
    }
}

