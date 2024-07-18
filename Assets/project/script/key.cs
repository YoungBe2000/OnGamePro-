using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class key : MonoBehaviour
{
    public DoorController objectWithBool;
    

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            objectWithBool.isConditionMet = true;
            Destroy(gameObject);
        }
    }
}
