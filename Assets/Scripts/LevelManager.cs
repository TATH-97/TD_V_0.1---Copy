using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
   public static LevelManager instance;
   [SerializeField] public GameObject[] spawnablePrefabs;

   [SerializeField] public GridLayout gameBoard; 

   private void Awake() {
        Debug.Log("GameManager");
        if(instance==null) {
            instance=this;
        }
    }

    public GameObject[] GetPrefabs() {
        return spawnablePrefabs;
    }
}
