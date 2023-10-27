using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerRuleSet : MonoBehaviour
{
    [SerializeField] public GameObject[] items;
    public bool[] abilitiesActive; //used for switching abilities on and off
    [SerializeField] int damage=20; //may not need, maybe apply to weapon
    [SerializeField] public GameObject weapon;

    


}
