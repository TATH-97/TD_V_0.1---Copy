using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class AttackerRuleSet : MonoBehaviour
{
    [SerializeField] public GameObject[] items;
    public bool[] abilitiesActive; //used for switching abilities on and off
    [SerializeField] int damage=20; //may not need, maybe apply to weapon
    [SerializeField] public GameObject weapon;
    [SerializeField] private SpriteRenderer sr;
    private float timeToAttack=1.5f;
    private float timeSenseLastAttack=1.5f;
    public bool isDead=false;
    public Transform home;

    public void Actions() {
        if(isDead) {
            gameObject.layer=9;
            sr.color= new Color(0.6981132f, 0.1541118f, 0.1541118f, 0.5019608f);
        }
    }
    



    void OnCollisionStay2D(Collision2D other) {
        GameObject go=other.gameObject;
        if(go.layer==8 && LevelManager.instance.isSpawning) {
            ItemHealth H=go.GetComponent<ItemHealth>();
            if(timeSenseLastAttack>=timeToAttack) {
                Debug.Log("Attack");
                H.TakeDamage(damage);
                timeSenseLastAttack=0;
            }
            timeSenseLastAttack+=Time.deltaTime;
        }
    }

    


}
