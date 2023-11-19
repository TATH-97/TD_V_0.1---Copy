using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GetFollowerScript : MonoBehaviour
{
    [SerializeField] private int minionLimit=3; 
    public int minionCount=0;
    private int level=1;
    private int levelLimit=2;
    private float radius=6f;
    public List<BasicMinionMovement> followers = new();

    public void GrabFollowers() {
        Collider2D[] hits=Physics2D.OverlapCircleAll(gameObject.transform.position, radius, 6);
        foreach(Collider2D col in hits) {
            GameObject temp =col.gameObject;
            if(!temp.GetComponent<NavMeshAgent>()) { //if not a minion
                continue;
            } else {
                if(minionCount<minionLimit) {
                    AddMinion(temp.gameObject);
                } else {
                    return;
                }
            }
        }
    }

    private void AddMinion(GameObject newMinion) {
        minionCount++;
        followers.Add(newMinion.GetComponent<BasicMinionMovement>());
        newMinion.GetComponent<BasicMinionMovement>().CMDJoinParty(gameObject.transform);
    }

    public void SetFree() {
        foreach(BasicMinionMovement minion in followers) {
            minionCount--;
            if(!minion) continue; //case where follower died
            minion.setTarget(minion.targetGameobject.transform);
            minion.CMDChangeLayer(6);
        }
    }

    public int GetMinionCount() { return minionCount; }
    public int GetLevel() { return level; }
    public void IncreaseLevel() {
        level++;
    }

}
