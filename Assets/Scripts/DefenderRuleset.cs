using UnityEngine;
using Mirror;

public class DefenderRuleset : NetworkBehaviour
{
    private GridLayout board;

    public void Inst() {
        GameObject[] objs=LevelManager.instance.GetPrefabs();
        foreach(GameObject gm in objs) {
            NetworkClient.RegisterPrefab(gm); //may need some kinda ID
        }
        board=LevelManager.instance.gameBoard;
    }

    public void ScreenMouseRay() {
        bool canBuild=true;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 5f;
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Collider2D[] col = Physics2D.OverlapPointAll(worldPosition);
        if(col.Length > 0){
            foreach(Collider2D c in col) {
                if(c.GetComponent<Collider2D>().gameObject.layer!=0) {
                    canBuild=false;
                }
            }
            if(canBuild) {
                Build(GetCellPos(worldPosition));
            } 
        }
    }   

    private Vector3 GetCellPos(Vector3 mousePosition) {
        Vector3Int gridPosition=board.WorldToCell(mousePosition);
        return board.CellToWorld(gridPosition);
    }
    
    private void Build(Vector3 pos) {  
        //spawn on the server
        CmdBuild(pos, Quaternion.identity);    
    } 

    [Command]
    private void CmdBuild(Vector3 pos, Quaternion rot) {
        GameObject towerToBuild = BuildManager.instance.GetSelectedTower();
        GameObject tower=Instantiate(towerToBuild);
        tower.transform.SetPositionAndRotation(pos, rot);
        NetworkServer.Spawn(tower);
    }
}
