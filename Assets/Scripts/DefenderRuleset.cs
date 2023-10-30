using UnityEngine;
using Mirror;

public class DefenderRuleset : NetworkBehaviour
{
    private GridLayout board;
    [SerializeField] private Sprite change;
    [SerializeField] private SpriteRenderer sr; 

    public void Inst() {
        board=LevelManager.instance.gameBoard;
        sr.sprite=change;
        sr.color= new Color(0.5660378f, 0.509434f, 509434f, 1); 
    }

    public void Actions() {
        if(Input.GetMouseButton(0)) {
            ScreenMouseRay();
        }
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
        Tower towerToBuild = BuildManager.instance.GetSelectedTower();
        if(towerToBuild==null) {
            return;
        }
        if(towerToBuild.cost<=BuildManager.instance.currency) {
            GameObject tower=Instantiate(towerToBuild.prefab);
            tower.transform.SetPositionAndRotation(pos, rot);
            NetworkServer.Spawn(tower);
            BuildManager.instance.currency-=towerToBuild.cost;
        } else {
            Debug.Log("Your broke");
        }
    }
}
