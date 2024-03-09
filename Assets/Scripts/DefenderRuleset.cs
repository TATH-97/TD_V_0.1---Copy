using UnityEngine;
using Mirror;

public class DefenderRuleset : NetworkBehaviour
{
    private GridLayout board;
    [SerializeField] private float FOV; 
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] public GameObject spottingSystem;
    [SerializeField] public GameObject Widget;
    public float moveSpeed= 0.075f;

    public void Inst() {
        board=LevelManager.instance.gameBoard;
        SetDrone();
        GameObject temp=GameObject.FindGameObjectWithTag("Vision"); //may need to synch over server
        temp.transform.localScale=new Vector3(8f, 8f, 1f);
        LevelManager.instance.SetFOW();  
    }

    private void FixedUpdate() {
        Movement();
    }

    public void Actions() {
        // Movement();
        if(Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftShift)) {
            BuildCheck(ScreenMouseRay(), ConvertToWorldPos(Input.mousePosition));
        }
        if(Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftShift)) {
            DeleteCheck(ScreenMouseRay());
        }
        if(Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("SPACE");
            CMDtest();
        }
    }

    //TEST CODE
    [Command] private void CMDtest() {
        GameObject test=Instantiate(Widget);
        test.transform.position=gameObject.transform.position;
        NetworkServer.Spawn(test);
        Debug.Log("CMDtest");
        RPCtest();
    }

    [ClientRpc] private void RPCtest() {
        if(isServer) {
            return;
        }
        Debug.Log("RPCTest");
        GameObject test=Instantiate(Widget);
        test.transform.position=gameObject.transform.position;
    }

    private Collider2D[] ScreenMouseRay() {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 5f;
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        return Physics2D.OverlapPointAll(worldPosition);
    }

    private Vector2 ConvertToWorldPos(Vector3 mousePosition) {
        mousePosition.z = 5f;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    } 

    private void DeleteCheck(Collider2D[] col) {
        if(col.Length>0) {
            foreach(Collider2D c in col) {
                if(c.gameObject.layer==8 && c.gameObject.tag!="Citadel") {
                    if(c.gameObject.tag.Equals("BeamTower")) {
                        if(!LevelManager.instance.isSpawning) {
                            BuildManager.instance.currency+=150;
                        } else {
                            BuildManager.instance.currency+=90;
                        }
                        NetworkServer.Destroy(c.gameObject);
                        return;
                    }
                    if(c.gameObject.tag.Equals("SpottingTower")) {
                        if(!LevelManager.instance.isSpawning) {
                            BuildManager.instance.currency+=80;
                        } else {
                            BuildManager.instance.currency+=50;
                        }
                        NetworkServer.Destroy(c.gameObject);
                        return;
                    } else {
                        if(!LevelManager.instance.isSpawning) {
                            BuildManager.instance.currency+=5;
                        } 
                        NetworkServer.Destroy(c.gameObject);
                    }
                }
            }
        }
    }

    private void BuildCheck(Collider2D[] col, Vector2 worldPosition) { //checks a given col array for canbuild
        bool canBuild=true;
        bool isSpawning=LevelManager.instance.isSpawning;
        if(col.Length > 0){//if we clicked on something
            if(!isSpawning) { //if is in round, no need to check vision
                foreach(Collider2D c in col) {
                    if((c.gameObject.layer!=0 && c.gameObject.layer!=9 && c.gameObject.layer!=7) || c.gameObject.tag=="Citadel") {
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
                    if(c.gameObject.layer==6 || c.gameObject.layer==8 || c.gameObject.layer==10) {
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
        BuildGen(pos); 
        CmdBuild(pos);    
    } 

    [Command(requiresAuthority =false)] private void CmdBuild(Vector3 pos) {
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
        BuildGen(pos);
    } 
    //*******************BUILD*******************\\

    //*******************DRONE*********************\\
    private void DroneGen() {
        sr.color= new Color(0.57f, 0.5f, 0.5f, 1); 
        gameObject.layer=9;
        gameObject.transform.position=new Vector3(0,0,0);
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
