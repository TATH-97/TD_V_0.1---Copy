using UnityEngine;

public class ItemHealth : MonoBehaviour
{
    // private BuildManager manager=BuildManager.instance;
    [SerializeField] public int startingHealth;
    private bool killed=false; //stop multiple calls

    public void TakeDamage(int damage) {
        if(killed) {
            return;
        }
        // Debug.Log("TakeDamage" + damage);
        startingHealth-=damage;
        if(startingHealth<=0) {
            killed=true;
            if(GetComponentInParent<BasicMinionMovement>()!=null) {
                LevelManager.onMinionKilled.Invoke();
                Destroy(gameObject);
                return;
            }
            if(GetComponentInParent<AttackerRuleSet>()!=null) {
                AttackerRuleSet ASS=GetComponentInParent<AttackerRuleSet>();
                ASS.isDead=true;
                return;
            } else {
                Destroy(gameObject);
            }
            // manager.DestroyBldg(gameObject); //maybe need for networking?
        }
    }
}
