using UnityEngine;
using Mirror;

public class BuildManager : NetworkBehaviour
{
    public static BuildManager instance;
    public bool armed=false;
    public bool wallArmed=false;

    // [SerializeField] private GameObject[] buildingPrefabs;
    [SerializeField] private Tower[] buildingPrefabs;
    public int currency;
    private int selectedTower=0;
    public Tower GetSelectedTower() {
        if(selectedTower==0 && wallArmed) {
            if(buildingPrefabs[selectedTower].cost>currency) {
                Debug.Log("Your broke!");
                return null;
            }
            // Cursor.SetCursor(buildingPrefabs[selectedTower].spriteRenderer.sprite, Vector2.zero, CursorMode.ForceSoftware);
            return buildingPrefabs[selectedTower];
        }
        if(armed && selectedTower!=0) {
            if(buildingPrefabs[selectedTower].cost>currency) {
                Debug.Log("Your broke!");
                return null;
            }
            wallArmed=false;
            armed=false;
            return buildingPrefabs[selectedTower];
        } else {
            return null;
        }      
    }

    public Tower[] GetPrefabs() {
        return buildingPrefabs;
    }
    
    private void Awake() {
        if(instance==null) {
            instance=this;
        }    
        Inst();    
    }

    public void ArmClicker() {
        armed=!armed;
    }

    public void ArmWallClicker() {
        wallArmed=!wallArmed;
    }

    public void DestroyBldg(GameObject bldg) {
        cmdDestroyBldg(bldg);
        Debug.Log("Destroy");
    }

    [ClientRpc]
    private void cmdDestroyBldg(GameObject bldg) {
        NetworkIdentity NI = bldg.GetComponent<NetworkIdentity>();
        Destroy(NI);
        Destroy(bldg);
    }

    public void SetSelectedTower(int idx) {
        Debug.Log("Selected Tower");
        selectedTower=idx;
    }

        
        private void Inst() {
        GameObject[] objs=LevelManager.instance.GetPrefabs();
        foreach(GameObject gm in objs) {
            NetworkClient.RegisterPrefab(gm); //may need some kinda ID
        }
    }

}
