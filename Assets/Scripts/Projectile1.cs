using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile1 : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float bulletVelosity=10f;
    [SerializeField] private int damageVal=1;
    private Transform target;
    private float Hx;
    private float Hy;

    public void SetTarget(Transform _target) {
        target=_target;
    }

    public void SetHome(Transform Home) {
        Hx=Home.transform.position.x;
        Hy=Home.transform.position.y;
    }

    private void FixedUpdate() {
        if(target==null) {
            Destroy(this);
            return;
        }
        Vector2 direction =target.position-transform.position;

        rb.velocity=direction * bulletVelosity;
        
    }

    private void OnCollisionEnter2D(Collision2D other) {
        //take health from minion. Will need diff function for players
        other.gameObject.GetComponent<MinionHealth>().TakeDamage(damageVal);
        Destroy(gameObject);  
    }

}
