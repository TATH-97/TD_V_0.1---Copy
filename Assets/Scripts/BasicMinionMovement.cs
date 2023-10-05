using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class BasicMinionMovement : NetworkBehaviour
{

    [SerializeField] Transform target;
    [SerializeField] Rigidbody2D rb;

    public void setTarget(Transform t) {
        target=t;
    }

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
    void FixedUpdate()
    {  
        if(UnityEngine.Vector3.Distance(dest, target.position)>1.0f) {
            dest = target.position;
            agent.destination = dest;
        }
    }
}
