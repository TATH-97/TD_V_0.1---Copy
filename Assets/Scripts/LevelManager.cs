using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
   public static LevelManager instance;

   private void Awake() {
        Debug.Log("GameManager");
        if(instance==null) {
            instance=this;
        }
    }
}
