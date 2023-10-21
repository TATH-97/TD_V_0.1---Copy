using UnityEngine;
using Mirror;

public class BuildManager : NetworkBehaviour
{
    public static BuildManager instance;

    // [SerializeField] private GameObject[] buildingPrefabs;
    [SerializeField] private Tower[] buildingPrefabs;

    private PlayerMovementController[] defendingPlayers;
    public int currency;
    private int selectedTower=0;
    public Tower GetSelectedTower() {
        return buildingPrefabs[selectedTower];
    }

    public Tower[] GetPrefabs() {
        return buildingPrefabs;
    }
    private void Awake() {
        if(instance==null) {
            instance=this;
        }        
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
        selectedTower=idx;
    }

}
