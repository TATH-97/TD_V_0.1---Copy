using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionHealth : MonoBehaviour
{
   [SerializeField] private int hitPoints=2;

    public void TakeDamage(int dmg) {
        hitPoints-=dmg;
        if(hitPoints<=0) {
            Destroy(gameObject);
        }
    }
}
