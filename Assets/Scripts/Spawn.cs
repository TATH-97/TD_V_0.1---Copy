using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Mirror;

public class Spawn : NetworkBehaviour {

    [SerializeField] int count;
    [SerializeField] GameObject spawnee;
    [SerializeField] Transform dest;

    // Start is called before the first frame update
    void Start()
    {
        spawn();
    }

    void spawn() {
        for(int i=0; i<count; i++) {
            Vector3 pos =new Vector3(-44.5f, 21.5f, 0f);
            cmdSpawn(pos, Quaternion.identity);
        }
    }

    [Command]
    void cmdSpawn(Vector3 pos, Quaternion rot) {
        GameObject newBorn=Instantiate(spawnee);
        BasicMinionMovement move =newBorn.GetComponent<BasicMinionMovement>();
        move.setTarget(dest);
        newBorn.transform.SetPositionAndRotation(pos, rot);
        NetworkServer.Spawn(newBorn);
    }
}
