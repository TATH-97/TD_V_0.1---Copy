using UnityEngine;
using Mirror;

public class BasicTower : NetworkBehaviour
{
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private float maxRange = 7f;
    [SerializeField] private float rotationSpeed=400f;
    [SerializeField] private float bps=1; //shots/sec
    [SerializeField] int damageVal=1; 
    
    [SyncVar] private Transform target; //need to add hook
    private float timeUntillFire;
    // private GameObject myBeam;
    [SerializeField] private LineRenderer lr;

    private void Update() {
        if(target==null) {
            lr.SetPosition(0, new Vector3(0,0,0));
            FindTarget();
            return;
        }
        if(target.gameObject.layer!=6 && target.gameObject.layer!=10) {
            FindTarget();
            return;     
        }
        RotateTowardsTarget(); 
        if(!ChecktargetIsInRange()) {
            target=null;
            if(!LevelManager.instance.isSpawning) { //if not in a wave dont shoot
                return;
            }
        } else {
            if(!LevelManager.instance.isSpawning) { //if not in a wave dont shoot
                return;
            }
            timeUntillFire+=Time.deltaTime;
            Shoot();
        }
    } 

    public void Shoot() {
        float angle=Mathf.Atan2((target.position.y - transform.position.y), (target.position.x - transform.position.x)) * Mathf.Rad2Deg - 90f;
        Vector3 pos=new Vector3(0, Vector2.Distance(firingPoint.position, target.position), 0);
        lr.SetPosition(0, pos);
        if(timeUntillFire>=1f/bps) {
            if(target.gameObject.layer==6 || target.gameObject.layer==10) {
                target.gameObject.GetComponent<ItemHealth>().TakeDamage(damageVal);
            }
            timeUntillFire=0f;
        }
    }

    private void FindTarget() {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, maxRange, (UnityEngine.Vector2)transform.position, 0f, enemyMask);
        if(hits.Length>0) {
            CMDUpdateTarget(hits[0].transform);
        }
    }

    [Command(requiresAuthority =false)] private void CMDUpdateTarget(Transform _target) {
        if(!_target) {
            target=null;
            return;
        }
        target=_target;
    }

    private bool ChecktargetIsInRange() {
        if(UnityEngine.Vector2.Distance(target.position, transform.position)>maxRange) {
            return false;
        } else {
            return true;
        }
    }

    private void RotateTowardsTarget() {
        float angle=Mathf.Atan2((target.position.y - transform.position.y), (target.position.x - transform.position.x)) * Mathf.Rad2Deg - 90f;
        UnityEngine.Quaternion targetRotation = UnityEngine.Quaternion.Euler(new UnityEngine.Vector3(0f, 0f, angle));
        turretRotationPoint.rotation=UnityEngine.Quaternion.RotateTowards(turretRotationPoint.rotation, targetRotation, rotationSpeed*Time.deltaTime);
    }

}
