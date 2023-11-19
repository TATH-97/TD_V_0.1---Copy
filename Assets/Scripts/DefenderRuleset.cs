using UnityEngine;
using Mirror;

public class DefenderRuleset : NetworkBehaviour
{
    private GridLayout board;
    [SerializeField] private float FOV; 
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] public GameObject spottingSystem;
    public float moveSpeed= 0.075f;

    public void Inst() {
        board=LevelManager.instance.gameBoard;
        SetDrone();
        GameObject temp=GameObject.FindGameObjectWithTag("Vision"); //may need to synch over server
        temp.transform.localScale=new Vector3(8f, 8f, 1f);
        LevelManager.instance.SetFOW();  
    }

    public void Actions() {
        Movement();
        if(Input.GetMouseButton(0)) {
            ScreenMouseRay();
        }
    }

    public void ScreenMouseRay() {
        bool canBuild=true;
        bool isSpawning=LevelManager.instance.isSpawning;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 5f;
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Collider2D[] col = Physics2D.OverlapPointAll(worldPosition);
        if(col.Length > 0){//if we clicked on something
            if(!isSpawning) { //if is in round, no need to check vision
                foreach(Collider2D c in col) {
                    if(c.GetComponent<Collider2D>().gameObject.layer!=0 && c.GetComponent<Collider2D>().gameObject.layer!=9) {
                        canBuild=false;
                        break;
                    }
                }
            } else {//if in round, need to check if we have vision
                canBuild=false;
                foreach(Collider2D c in col) {
                    if(c.gameObject.tag=="Vision") {
                        canBuild=true;
                    }
                    if(c.GetComponent<Collider2D>().gameObject.layer==6 || c.GetComponent<Collider2D>().gameObject.layer==8) {
                        canBuild=false;
                        break;
                    }
                }
            }
            if(canBuild) {
                CmdBuild(GetCellPos(worldPosition));
            } 
        }
    }   

    private Vector3 GetCellPos(Vector3 mousePosition) {
        Vector3Int gridPosition=board.WorldToCell(mousePosition);
        return board.CellToWorld(gridPosition);
    }

    //*******************BUILD*******************\\
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
        GameObject tower=Instantiate(towerToBuild.prefab);
        tower.transform.SetPositionAndRotation(pos, Quaternion.identity);
        NetworkServer.Spawn(tower);
        // RPCSpawnTower(pos);
        BuildManager.instance.currency-=towerToBuild.cost;      
    }

    [ClientRpc] private void RPCSpawnTower(Vector3 pos) {
        Debug.Log("RPCSpawnTower");
        BuildGen(pos);
    }
    //*******************BUILD*******************\\

    //*******************DRONE*********************\\
    private void DroneGen() {
        sr.color= new Color(0.57f, 0.5f, 0.5f, 1); 
        gameObject.layer=9;
    }

    [Client] private void SetDrone() {
        if(!isLocalPlayer) return;
        DroneGen();
        CMDSetDrone();
    }

    [Command] private void CMDSetDrone() {
        DroneGen();
        RPCSetDrone();
    }

    [ClientRpc] private void RPCSetDrone() {
        if(isLocalPlayer) {
            return;
        }
        DroneGen();
    }
    //*******************DRONE*********************\\


    public void Movement() {
        float xDirection=Input.GetAxis("Horizontal"); 
        float yDirection= Input.GetAxis("Vertical");
        Vector3 moveDirection =new Vector3(xDirection, yDirection, 0.0f);
        transform.position += moveDirection * moveSpeed;
    }
}
