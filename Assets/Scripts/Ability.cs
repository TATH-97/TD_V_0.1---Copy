using System;
using UnityEngine;

[Serializable]
public class Ability 
{
    public GameObject prefab;
    public string name; 

    public Ability(string _name, GameObject _prefab) {
        name=_name;
        prefab=_prefab;
    }
}
