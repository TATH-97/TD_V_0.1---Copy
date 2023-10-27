using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class BasicMinionMovement : NetworkBehaviour
{
    [SerializeField] public GameObject _target;
    [SerializeField] public Transform target; 
    [SerializeField] Rigidbody2D rb;
    [SerializeField] int damage;

    public void setTarget(Transform t) {
        target=t;
    }

    NavMeshAgent agent;
    UnityEngine.Vector3 dest;
    // Start is called before the first frame update
    void Start()	{
        // target = _target.transform;
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
            // Debug.Log(transform.position.ToString());
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        GameObject go=other.gameObject;
        if(go.layer==8) {
            // Debug.Log("Do damage");
            ItemHealth H=go.GetComponent<ItemHealth>();
            H.TakeDamage(damage);
        }
    }
}
