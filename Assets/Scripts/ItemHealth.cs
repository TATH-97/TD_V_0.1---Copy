using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class ItemHealth : NetworkBehaviour  
{
    [SerializeField] public int startingHealth;
    [SyncVar] public int health;
    private bool killed=false; //stop multiple calls

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
            }
            if(GameObject.FindWithTag("Citadel")) {
                Debug.Log("EndGame");
            } else {
                NetworkServer.Destroy(gameObject);
                // Destroy(gameObject);
            }
            // manager.DestroyBldg(gameObject); //maybe need for networking?
        }
    }

    public void AddHealth(int more) {
        health+=more;
    } 

    public void ResetHealth() {
        health=startingHealth;
    }
}
