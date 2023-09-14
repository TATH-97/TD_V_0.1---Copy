using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Mirror;

public class Spawn : MonoBehaviour {

    [SerializeField] int count;
    [SerializeField] GameObject spawnee;
    [SerializeField] Transform dest;
   
    //private GameObject newBorn;

    // Start is called before the first frame update
    void Start()
    {
        spawn();
    }

    void spawn() {
        for(int i=0; i<count; i++) {
            GameObject newBorn=Instantiate(spawnee) as GameObject;
            BasicMinionMovement move=newBorn.GetComponent<BasicMinionMovement>();
            move.setTarget(dest);
        }
    }
}
