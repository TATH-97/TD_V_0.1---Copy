using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{   
    public static AbilityManager instance;
    [SerializeField] public GameObject[] attackerAbilities;
    private int idex;
    private void Awake() {
        if(instance==null) {
            instance=this;
        }
    }

    private void SetIndex(int _idex) {
        idex=_idex;
    }

    private GameObject GetAbility() {
        return attackerAbilities[idex];
    } 
}
