using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{   
    public static AbilityManager instance;
    [SerializeField] public Ability[] attackerAbilities;
    private int idex;
    private void Awake() {
        if(instance==null) {
            instance=this;
        }
    }

    private void SetIndex(int _idex) {
        idex=_idex+1;
    }

    private Ability GetAbility() {  
        return attackerAbilities[idex];
    } 
}
