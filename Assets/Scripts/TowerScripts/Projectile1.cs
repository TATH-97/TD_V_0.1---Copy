using UnityEngine;

public class Projectile1 : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float bulletVelosity=10f;
    [SerializeField] private int damageVal=1;
    [SerializeField] private int dist;
    private Transform target;
    private Vector3 home;

    public void SetTarget(Transform _target) {
        rb.mass=0;
        target=_target;
    }

    public void SetHome(Transform Home) {
        home=Home.position;
    }

    private void FixedUpdate() {
        if(target==null) {
            // Debug.Log("POP");
            Destroy(gameObject);
            Destroy(this);
            return;
        }
        if(Vector3.Distance(this.transform.position, home)>dist) {
            Destroy(gameObject);
            Destroy(this);
            return;
        }
        Vector2 direction =target.position-transform.position;
        rb.velocity=direction * bulletVelosity; 
        if(target.gameObject.layer!=6) {
            Destroy(gameObject);
        }   
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if((other.gameObject.GetComponent<ItemHealth>()!=null && other.gameObject.layer==6) ||
        other.gameObject.GetComponent<ItemHealth>()!=null && other.gameObject.layer==10) {
            other.gameObject.GetComponent<ItemHealth>().TakeDamage(damageVal);
            Destroy(gameObject); 
        } else {
            // Debug.Log("OUCH!");
        }  
    }
}
