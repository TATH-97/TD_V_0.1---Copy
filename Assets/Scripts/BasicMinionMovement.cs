using UnityEngine;
using UnityEngine.AI;
using Mirror;
using System.Collections.Generic;
using System.Collections;

public class BasicMinionMovement : NetworkBehaviour
{
    [SerializeField] public GameObject _target;
    [SerializeField] public Transform target; 
    [SerializeField] Rigidbody2D rb;
    [SerializeField] int damage;
    [SerializeField] private float timeToAttack=1.5f;
    [SerializeField] private float timeSenseLastAttack=1.5f;
    NavMeshAgent agent;
    UnityEngine.Vector3 dest;
    

    public void setTarget(Transform t) {
        target=t;
    }
    
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

    void OnCollisionStay2D(Collision2D other) {
        GameObject go=other.gameObject;
        if(go.layer==8) {
            ItemHealth H=go.GetComponent<ItemHealth>();
            if(timeSenseLastAttack>=timeToAttack) {
                H.TakeDamage(damage);
                timeSenseLastAttack=0;
            }
            timeSenseLastAttack+=Time.deltaTime;
        }
    }
}
