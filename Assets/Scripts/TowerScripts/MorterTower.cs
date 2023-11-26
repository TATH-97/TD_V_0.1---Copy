using Mirror;
using UnityEngine;

public class MorterTower : NetworkBehaviour
{
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private float maxRange = 15f;
    [SerializeField] private float rotationSpeed=400f;
    [SerializeField] private float bps=0.8f; //shots/sec
    
    [SyncVar] public Transform target; //need to add hook
    private float timeUntillFire=100f;
    
    
    private void Update() {
        if(target==null) {
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
            if(timeUntillFire>=1f/bps) {
                Shoot();
                timeUntillFire=0f;
                FindTarget();
            }
        }
    }

    public void Shoot() {
        float angle=Mathf.Atan2((target.position.y - transform.position.y), (target.position.x - transform.position.x)) * Mathf.Rad2Deg - 90f;
        UnityEngine.Quaternion targetRotation = UnityEngine.Quaternion.Euler(new UnityEngine.Vector3(0f, 0f, angle));
        GameObject bullet= Instantiate(projectilePrefab, firingPoint.position, targetRotation);
        AOEProjectile bulletScript = bullet.GetComponent<AOEProjectile>();
        bulletScript.SetHome(transform);
        bulletScript.SetTarget(target); 
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
