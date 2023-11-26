using UnityEngine;

public class AOEProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float bulletVelosity=2f;
    [SerializeField] GameObject explosion;
    [SerializeField] float minRange=1.5f;
    private Vector3 direction;
    private GameObject marker;
    private float distanceTraveled=0;
    private Transform home;
    private float maxDistance;
    private bool saftey=false;

    public void SetTarget(Transform _target) {
        marker=new GameObject();
        marker.transform.position=_target.position;
        direction=(marker.transform.position-transform.position).normalized;
        maxDistance=Vector2.Distance(home.position, marker.transform.position);
        if(maxDistance<minRange) {
            saftey=true;
        }
    }

    public void SetHome(Transform _Home) {
        home=_Home;
    }

    private void FixedUpdate() {
        if(saftey) {
            distanceTraveled=Vector2.Distance(transform.position, home.position);
            if(distanceTraveled>=minRange) {
                BOOM();
                return;
            } else {
                 rb.velocity=direction * bulletVelosity;
                return;
            }
        }
        if(Vector2.Distance(marker.transform.position, transform.position)<=.25f) {
            BOOM();
        }
        rb.velocity=direction * bulletVelosity;   
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if((other.gameObject.GetComponent<ItemHealth>()!=null && other.gameObject.layer==6) ||
        other.gameObject.GetComponent<ItemHealth>()!=null && other.gameObject.layer==10) {
            BOOM();
        } 
    }

    private void BOOM() {
        GameObject boom= Instantiate(explosion);
        boom.transform.position=transform.position;
        Destroy(marker);
        Destroy(gameObject);
    }
}
