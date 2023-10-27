using UnityEngine;

public class ItemHealth : MonoBehaviour
{
    private BuildManager manager=BuildManager.instance;
    [SerializeField] public int startingHealth;

    public void TakeDamage(int damage) {
        startingHealth-=damage;
        if(startingHealth<=0) {
            if(GetComponentInParent<BasicMinionMovement>()!=null) {
                LevelManager.onMinionKilled.Invoke();
            }
            // manager.DestroyBldg(gameObject); //maybe need for networking?
            Destroy(gameObject);
        }
    }
}
