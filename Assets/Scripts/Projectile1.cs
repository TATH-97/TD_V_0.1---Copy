using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile1 : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float bulletVelosity=10f;
    [SerializeField] private int damageVal=1;
    [SerializeField] private int dist;
    private Transform target;
    private float Hx;
    private float Hy;
    private Vector3 home;
    private GameObject parent;

    public void SetTarget(Transform _target) {
        target=_target;
    }

    public void SetHome(Transform Home) {
        home=Home.position;
    }

    public void SetParent(GameObject p) {
        parent=p;
    }

    private void FixedUpdate() {
        if(target==null) {
            Debug.Log("POP");
            Destroy(parent);
            Destroy(this);
            return;
        }
        if(Vector3.Distance(this.transform.position, home)>=dist) {
            Destroy(this);
            return;
        }
        Vector2 direction =target.position-transform.position;

        rb.velocity=direction * bulletVelosity;    
    }

    private void OnCollisionEnter2D(Collision2D other) {
        //take health from minion. Will need diff function for players
        if(other.gameObject.GetComponent<MinionHealth>()!=null) {
            other.gameObject.GetComponent<MinionHealth>().TakeDamage(damageVal);
            Destroy(gameObject); 
        } else {
            Debug.Log("OUCH!");
        }  
    }
}
