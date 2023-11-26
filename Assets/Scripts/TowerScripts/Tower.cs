using System;
using UnityEngine;

[Serializable]
public class Tower
{
    public int cost;
    public GameObject prefab;
    public string name; 
    public SpriteRenderer spriteRenderer;

    public Tower(string _name, int _cost, GameObject _prefab) {
        name=_name;
        cost=_cost;
        prefab=_prefab;
        spriteRenderer=prefab.GetComponent<SpriteRenderer>();
    }

    public static explicit operator GameObject(Tower v)
    {
        throw new NotImplementedException();
    }
}
