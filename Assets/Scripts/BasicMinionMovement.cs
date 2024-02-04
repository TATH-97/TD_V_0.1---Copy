using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class BasicMinionMovement : NetworkBehaviour
{
    [SerializeField] public GameObject targetGameobject;
    [SerializeField] public Transform target; 
    [SerializeField] Rigidbody2D rb;
    [SerializeField] public int damage;
    [SerializeField] private float timeToAttack=1.5f;
    [SerializeField] private float timeSenseLastAttack=1.5f;
    NavMeshAgent agent;
    UnityEngine.Vector3 dest;
    private bool cast=false;
    GameObject marker;
    

    public void setTarget(Transform t) {
        target=t;
    }
    
    void Start() {
		agent = GetComponent<NavMeshAgent>();
		agent.updateRotation = false;
		agent.updateUpAxis = false;
        dest = agent.destination;
        setTarget(targetGameobject.transform);
	}

    // Update is called once per frame
    void FixedUpdate()
    {  
        if(UnityEngine.Vector3.Distance(dest, target.position)>2f && cast) {
            cast=false;
            CMDSetFree();
            Destroy(marker);
        }
        if(UnityEngine.Vector3.Distance(dest, target.position)>1.0f) { //issue
            dest = target.position;
            agent.destination = dest;
        }
       
    }

    void OnCollisionStay2D(Collision2D other) {
        GameObject go=other.gameObject;
        if(go.layer!=8) {
            return;
        }
        if(go.layer==8) {
            ItemHealth H=go.GetComponent<ItemHealth>();
            if(timeSenseLastAttack>=timeToAttack) {
                H.TakeDamage(damage);
                timeSenseLastAttack=0;
            }
            timeSenseLastAttack+=Time.deltaTime;
        }
    }

    [Command(requiresAuthority =false)] public void CMDJoinParty(Transform newTarget) {
        setTarget(newTarget);
        // CMDChangeLayer(10);
        gameObject.GetComponent<NavMeshAgent>().speed=8;
    }

    [Command(requiresAuthority =false)] public void CMDCast(GameObject newTarget) {
        setTarget(newTarget.transform);
        // CMDChangeLayer(10);
        gameObject.GetComponent<NavMeshAgent>().speed=8;
        cast=true;
        marker=newTarget;
    }

    [Command(requiresAuthority =false)] public void CMDSetFree() {
        gameObject.GetComponent<NavMeshAgent>().speed=2;
        setTarget(targetGameobject.transform);
        // CMDChangeLayer(6);
    }

    [Command(requiresAuthority =false)] public void CMDChangeLayer(int layer) {
        gameObject.layer=layer;
        RPCChangeLayer(layer);
    }

    [ClientRpc] private void RPCChangeLayer(int layer) {
        if(isLocalPlayer) {return;}
        gameObject.layer=layer;
    }
}
