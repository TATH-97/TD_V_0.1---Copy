using UnityEngine;

public class ItemHealth : MonoBehaviour
{
    private BuildManager manager=BuildManager.instance;
    [SerializeField] public int startingHealth;

    public void TakeDamage(int damage) {
        Debug.Log("Take "+damage);
        startingHealth-=damage;
        if(startingHealth<=0) {
            // manager.DestroyBldg(gameObject); //maybe need for networking?
            Destroy(gameObject);
        }
    }
}
