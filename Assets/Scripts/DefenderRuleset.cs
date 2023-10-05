using NavMeshPlus.Components;
using UnityEngine;
using Mirror;

public class DefenderRuleset : NetworkBehaviour
{
    private NavMeshSurface navMesh;
    PlayerObjectController parent; 

    public void SetParent(PlayerObjectController p) {
        parent=p;
        navMesh=GameObject.Find("NavMesh").GetComponent<NavMeshSurface>();
        GameObject[] objs=BuildManager.instance.GetPrefabs();
        foreach(GameObject gm in objs) {
            NetworkClient.RegisterPrefab(gm); //may need some kinda ID
        }
    }

    public void ScreenMouseRay() {
        bool canBuild=true;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 5f;
 
        Vector2 v = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 pos=Camera.main.ScreenToWorldPoint(mousePosition);
        pos.z=0f;
        Collider2D[] col = Physics2D.OverlapPointAll(v);
 
        if(col.Length > 0){
            foreach(Collider2D c in col) {
                if(c.GetComponent<Collider2D>().gameObject.layer!=0) {
                    canBuild=false;
                }
            }
            if(canBuild) {
                Build(pos);
            } else {
                // Debug.Log("Cant build");
            }   
        }
    }   
    
    private void Build(Vector3 pos) {  
        //spawn on the server
        CmdBuild(pos, Quaternion.identity);    
    } 

    [Command]
    private void CmdBuild(Vector3 pos, Quaternion rot) {
        GameObject towerToBuild = BuildManager.instance.getSelectedTower();
        GameObject tower=Instantiate(towerToBuild);
        tower.transform.SetPositionAndRotation(pos, rot);
        NetworkServer.Spawn(tower);
        //Update pathfinding
        if(tower.GetComponent<NavMeshModifier>()) { //dosent update AI. 
            Debug.Log("ReBake");
            navMesh.BuildNavMesh();
        }
    }
}
