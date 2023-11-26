using System;
using UnityEngine;

public class AOEExplosion : MonoBehaviour
{
    private SpriteRenderer sr;
    private float frameTimeMarker=.1875f;
    private float frameTime=0f; 
    private int timesChanged=0;
    [SerializeField] LayerMask mask;
    private bool DoDamage=true;
    private void Awake() {
        sr=GetComponent<SpriteRenderer>();
    }
    private void FixedUpdate() {
        if(timesChanged==1 && DoDamage) {
            DODamage();
            DoDamage=false;
        }
        frameTime+=Time.deltaTime;
        if(frameTime>=frameTimeMarker) {
            timesChanged++;
            sr.color=new Color(.8f,.55f,.2f,(float)(.5-(.05*timesChanged)));
            frameTime=0;
        }
        if(.5-(.08*timesChanged)<.1) {
            Destroy(gameObject);
        }
    }

    private void DODamage() {
        float radius=gameObject.transform.lossyScale.x/2;
        Collider2D[] hits=Physics2D.OverlapCircleAll(gameObject.transform.position, radius, mask);
        if(hits.Length==0) {
            return;
        } 
        Debug.Log("BANG");
        foreach(Collider2D col in hits) {
            int damageVal=CalculateDamageByDist(col);
            Debug.Log(damageVal.ToString());
            col.gameObject.GetComponent<ItemHealth>().TakeDamage(damageVal);
        }
    }

    private int CalculateDamageByDist(Collider2D col) {
        double dist=Math.Abs(Vector2.Distance(transform.position, col.transform.position));
        // dist=Math.Round(dist);
        if(dist<=.5) {
            return 20;
        }
        if(dist<=1) {
            return 15;
        }
        if(dist<=3) {
            return 10;
        }
        else return 5;
    }
}
