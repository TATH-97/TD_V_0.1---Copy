using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;

public class BasicMinionMovement : MonoBehaviour
{

    [SerializeField] Transform target;
    NavMeshAgent agent;
    UnityEngine.Vector3 dest;
    // Start is called before the first frame update
    void Start()	{
		agent = GetComponent<NavMeshAgent>();
		agent.updateRotation = false;
		agent.updateUpAxis = false;
        dest = agent.destination;
	}

    // Update is called once per frame
    void Update()
    {   
        if(UnityEngine.Vector3.Distance(dest, target.position)>1.0f) {
            dest = target.position;
            agent.destination = dest;
        }
    }
}
