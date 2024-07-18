using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public GameObject Enemycheck;
    public Transform doorTransform;
    public float openAngle = 55f;
    public float closeDelay = 1f;
    public float rotationSpeed = 5f;

    private bool isOpening = false;
    private bool isClosing = false;
    private float initialYRotation;

    public key objectWithBool;
    public bool isConditionMet = false;


    public EnemyAi enemyTr;

    void Start()
    {
        initialYRotation = doorTransform.eulerAngles.y;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isOpening && !isClosing)
            {
                if (isConditionMet == true)
                {
                    if (!isOpening && !isClosing)
                    {
                        StartCoroutine(OpenDoor());
                    }
                }
            }
        }

        if (other.CompareTag("Enemy"))
        {
            if (enemyTr.enemy == false)
            {
                enemyTr.enemy = true;
            }
            else
            {
                enemyTr.enemy = false;
            }
           
        }
        

    }

    IEnumerator OpenDoor()
    {
        isOpening = true;
        float targetRotation = initialYRotation + openAngle;

        while (doorTransform.eulerAngles.y < targetRotation)
        {
            doorTransform.rotation = Quaternion.RotateTowards(doorTransform.rotation, Quaternion.Euler(0, targetRotation, 0), rotationSpeed * Time.deltaTime);
            yield return null;
        }

        doorTransform.rotation = Quaternion.Euler(0, targetRotation, 0);
        isOpening = false;

        yield return new WaitForSeconds(closeDelay);

        StartCoroutine(CloseDoor());
    }

    IEnumerator CloseDoor()
    {
        isClosing = true;
        float targetRotation = initialYRotation;

        while (doorTransform.eulerAngles.y > targetRotation)
        {
            doorTransform.rotation = Quaternion.RotateTowards(doorTransform.rotation, Quaternion.Euler(0, targetRotation, 0), rotationSpeed * Time.deltaTime);
            yield return null;
        }

        doorTransform.rotation = Quaternion.Euler(0, targetRotation, 0);
        isClosing = false;
    }
}

