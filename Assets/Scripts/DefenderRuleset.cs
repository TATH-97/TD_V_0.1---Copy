using UnityEngine;
using Mirror;

public class DefenderRuleset : NetworkBehaviour
{
    private GridLayout board;
    [SerializeField] private SpriteRenderer sr;
    // private Color col=new Color(0.57f, 0.5f, 0.5f, 1); 

    public void Inst() {
        board=LevelManager.instance.gameBoard;
        SetDrone();
    }

//     IEnumerator WaitForReady() {
//     while (!connectionToClient.isReady) {
//         yield return new WaitForSeconds(0.25f);
//     }
//     board=LevelManager.instance.gameBoard;
//     SetDrone();
// }

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
                if(c.GetComponent<Collider2D>().gameObject.layer!=0 && c.GetComponent<Collider2D>().gameObject.layer!=9) {
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

    //*******************BUILD*******************
    private void BuildGen(Vector3 pos) {
        Tower towerToBuild = BuildManager.instance.GetSelectedTower();
        if(towerToBuild==null) {
            return;
        }
        if(towerToBuild.cost<=BuildManager.instance.currency) {
            GameObject tower=Instantiate(towerToBuild.prefab);
            tower.transform.SetPositionAndRotation(pos, Quaternion.identity);
            BuildManager.instance.currency-=towerToBuild.cost;
        }
    }
    
    [Client] private void Build(Vector3 pos) { 
        if(!isLocalPlayer) return; 
        Debug.Log("Build");
        BuildGen(pos); 
        CmdBuild(pos);    
    } 

    [Command(requiresAuthority =false)] private void CmdBuild(Vector3 pos) {
        Debug.Log("CmdBuild");
        Tower towerToBuild = BuildManager.instance.GetSelectedTower();
        if(towerToBuild==null) {
            return;
        }
        if(towerToBuild.cost<=BuildManager.instance.currency) {
            GameObject tower=Instantiate(towerToBuild.prefab);
            tower.transform.SetPositionAndRotation(pos, Quaternion.identity);
            NetworkServer.Spawn(tower);
            // RPCSpawnTower(pos);
            BuildManager.instance.currency-=towerToBuild.cost;
        } else {
            Debug.Log("Your broke");
        }
    }

    [ClientRpc] private void RPCSpawnTower(Vector3 pos) {
        Debug.Log("RPCSpawnTower");
        BuildGen(pos);
    }
    //*******************BUILD*******************

    //*******************DRONE*********************
    private void DroneGen() {
        sr.color= new Color(0.57f, 0.5f, 0.5f, 1); 
        gameObject.layer=9;
    }

    [Client] private void SetDrone() {
        if(!isLocalPlayer) return;
        Debug.Log("SetDrone");
        DroneGen();
        CMDSetDrone();
    }

    [Command] private void CMDSetDrone() {
        Debug.Log("CMDSetDrone");
        DroneGen();
        RPCSetDrone();
    }

    [ClientRpc] private void RPCSetDrone() {
        if(isLocalPlayer) {
            return;
        }
        Debug.Log("RPCSetDrone");
        DroneGen();
    }
    //*******************DRONE*********************
}
