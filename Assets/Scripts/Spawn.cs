using UnityEngine;
using Mirror;
using System.Collections;

public class Spawn : MonoBehaviour {

    [SerializeField] GameObject[] spawnees; //list of possable spawnees
    [SerializeField] Transform dest;
    private ArrayList minions = new ArrayList();

    private int idex;


    public void spawn() {  
        cmdSpawn(transform.position, Quaternion.identity);
    }

    //[Command(requiresAuthority = false)]
    void cmdSpawn(Vector3 pos, Quaternion rot) {
        GameObject newBorn=Instantiate(spawnees[idex]);
        BasicMinionMovement move =newBorn.GetComponent<BasicMinionMovement>();
        move.setTarget(dest);
        newBorn.transform.SetPositionAndRotation(pos, rot);
        NetworkServer.Spawn(newBorn);
        minions.Add(newBorn);
    }
}
