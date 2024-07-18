using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DoorDetect : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어는 통과할 수 있도록 아무 것도 하지 않음
        }
        else if (other.CompareTag("Enemy"))
        {
            // 적은 통과하지 못하도록 설정
            other.GetComponent<Collider>().isTrigger = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // 적이 영역을 벗어나면 다시 Trigger로 변경
            other.GetComponent<Collider>().isTrigger = true;
        }
    }
}

