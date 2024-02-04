using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GetFollowerScript : MonoBehaviour
{
    [SerializeField] public int minionLimit=3; 
    public int minionCount=0;
    private int level=1;
    private int levelLimit=3;
    private float radius=6;
    public List<BasicMinionMovement> followers = new();
    public float coolDownTime=15;

    public void GrabFollowers() {
        if(minionCount>=minionLimit) return;
        Collider2D[] hits=Physics2D.OverlapCircleAll(gameObject.transform.position, radius, LayerMask.GetMask("Minions"));
        if(hits.Length==0) {
            return;
        }
        foreach(Collider2D col in hits) {
            GameObject temp = col.gameObject;
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
        if(minionCount>=minionLimit) return;
        minionCount++;
        followers.Add(newMinion.GetComponent<BasicMinionMovement>());
        newMinion.GetComponent<BasicMinionMovement>().CMDJoinParty(gameObject.transform);
    }

    public void SetFree() {
        foreach(BasicMinionMovement minion in followers) {
            if(!minion) continue; //case where follower died
            minion.CMDSetFree();
            minionCount=0;
        }
    }

    public void CastMinions(GameObject marker) {
        foreach(BasicMinionMovement minion in followers) {
            if(!minion) continue; //case where follower died
            minion.CMDCast(marker);
            minionCount=0;
        }
    }

    public void LevelUp() {
        if(level>=levelLimit) { //remove when fully patched
            return;
        }
        level++;
        radius+=level*2;
        minionLimit*=2;
        if(level==levelLimit) {
            coolDownTime=8f;
        }
    }

    public int GetMinionCount() { return minionCount; }
    public int GetLevel() { return level; }
    public void IncreaseLevel() {
        level++;
    }

}
