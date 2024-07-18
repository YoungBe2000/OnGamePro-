using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Mon_control : MonoBehaviour
{
    public NavMeshAgent myAgent;
    public Transform targetTr;

    // Start is called before the first frame update
    void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        myAgent.SetDestination(targetTr.position);
        
    }
}
