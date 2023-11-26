using Mirror;
using UnityEngine;

public class ItemHealth : NetworkBehaviour  
{
    [SerializeField] public int startingHealth;
    [SyncVar] public int health;
    public bool killed=false; //stop multiple calls

    private void Awake() {
        health=startingHealth;
    }

    public void TakeDamage(int damage) {
        if(killed) {
            return;
        }
        // Debug.Log("TakeDamage" + damage);
        health-=damage;
        if(health<=0) {
            killed=true;
            if(GetComponentInParent<BasicMinionMovement>()) {
                LevelManager.onMinionKilled.Invoke();
                Destroy(gameObject);
                return;
            }
            if(GetComponentInParent<AttackerRuleSet>()) {
                AttackerRuleSet ASS=GetComponentInParent<AttackerRuleSet>();
                ASS.isDead=true;
                return;
            } else {
                if(isServer) {
                    NetworkServer.Destroy(gameObject);
                } else {
                    ClientDestroy();
                }
            }
        }
    }

    [Command(requiresAuthority =false)] private void ClientDestroy() {
        NetworkServer.Destroy(gameObject);
    } 

    public void AddHealth(int more) {
        health+=more;
    } 

    public void ResetHealth() {
        health=startingHealth;
    }

    public void ResetKilled() {
        killed=false;
    }   
}
