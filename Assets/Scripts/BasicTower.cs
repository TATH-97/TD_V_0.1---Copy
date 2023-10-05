using UnityEngine;
using UnityEditor;
// using Mirror;

public class BasicTower : MonoBehaviour
{
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject beamPrefab;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private float maxRange = 10f;
    [SerializeField] private float rotationSpeed=400f;
    [SerializeField] private float bps=1; //shots/sec
    
    private Transform target;
    private float timeUntillFire;

    private void Update() {
        if(target==null) {
            FindTarget();
            return;
        }
        RotateTowardsTarget(); 
        if(!ChecktargetIsInRange()) {
            target=null;
        } else {
            timeUntillFire+=Time.deltaTime;
            if(timeUntillFire>=1f/bps) {
                Shoot();
                timeUntillFire=0f;
            }
        }
    } 

    public void Shoot() {
        GameObject bullet= Instantiate(beamPrefab, firingPoint.position, UnityEngine.Quaternion.identity);
        Projectile1 bulletScript = bullet.GetComponent<Projectile1>();
        bulletScript.SetTarget(target); 
        bulletScript.SetHome(transform);
        bulletScript.SetParent(bullet);
    }

    private void FindTarget() {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, maxRange, (UnityEngine.Vector2)transform.position, 0f, enemyMask);
        if(hits.Length>0) {
            target=hits[0].transform;
        }
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

    // private void OnDrawGizmosSelected() {
    //     Handles.color=Color.cyan;
    //     Handles.DrawWireDisc(transform.position, transform.forward, maxRange);

    // }
}