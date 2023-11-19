using UnityEngine;
using Mirror;

public class AttackerRuleSet : NetworkBehaviour
{
    public bool[] abilitiesActive; //used for switching abilities on and off
    [SerializeField] int damage=20; //may not need, maybe apply to weapon
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private GameObject spottingSystemPrefab;
    // [SerializeField] private GameObject grabMinions;
    [SyncVar] private Color col=new Color(1,0,0,1);
    private float timeToAttack=1.5f;
    private float timeSenseLastAttack=1.5f;
    public bool isDead=false;
    private bool changed=false;
    public float moveSpeed= 0.1f;
    public Transform home; //default
    private SpriteMask mask; 
    // private GameObject minionGrabber;

    public void Inst() {
        CLFogOfWarGen();
        LevelManager.instance.SetFOW();
        // minionGrabber=Instantiate(grabMinions);
    }

    public void Actions() {
        Movement();
        if(!LevelManager.instance.isSpawning) { //if not in a round
            if(LevelManager.instance.timeBetweenWaves-LevelManager.instance.lastWaveTime<=1.5f) { //about to start round
                if(!home) {
                    home=LevelManager.instance.spawners[0].transform;    
                }
                gameObject.transform.position=home.position;
                Respawn();
            }

            if(Input.GetMouseButton(0)) {
                ScreenMouseRay();
            }
            if(Input.GetKeyDown(KeyCode.Alpha1)) {
                
            }
        } 
        
        else { //if in a round
            if(isDead && !changed) {
                PlayerDead();
            }
        }
    }

    //**************DEATH*******************\\
    private void DeathGen() {
        changed=true;
        gameObject.layer=9;
        col=new Color (0.7f, 0.15f, 0.15f, 0.5f);
        sr.color= col;
    }
    
    [Client] private void PlayerDead() {
        if(!isLocalPlayer) return;
        DeathGen();
        CMDPlayerDead();
    }

    [Command] private void CMDPlayerDead() {
        DeathGen();
        RPCPlayerDead();
    }

    [ClientRpc] private void RPCPlayerDead() {
        if(isLocalPlayer) return;
        DeathGen();
    }
    //**************DEATH*******************\\


    //******************FogOfWar******************\\
    private void FogOfWarGen() {
        // GameObject temp=GameObject.FindWithTag("Vision");
        // mask=temp.GetComponent<SpriteMask>();
        mask=gameObject.GetComponentInChildren<SpriteMask>();
        mask.frontSortingOrder=0;
        GameObject maskA=Instantiate(spottingSystemPrefab);
        maskA.transform.parent=gameObject.transform;
        maskA.transform.position=gameObject.transform.position;
    }

    [Client] private void CLFogOfWarGen() {
        if(!isLocalPlayer) return;
        Debug.Log("CLFogOfWarGen");
        FogOfWarGen();
        CMDFogOfWar(); 
    }

    [Command] private void CMDFogOfWar() {
        Debug.Log("CMDFogOfWar");
        FogOfWarGen();
        RPCFogOfWar();
    }

    [ClientRpc] private void RPCFogOfWar() {
        if(isLocalPlayer) return;
        Debug.Log("RPCFogOfWar");
        FogOfWarGen();
    }
    //******************FogOfWar******************\\

    
    //******************RESPAWN******************\\
    private void RespawnGen() {
        isDead=false;
        changed=false;
        gameObject.layer=6;
        col=new Color(1f, 0.0f, 0.0f, 1);
        sr.color= col;
        ItemHealth h =GetComponentInParent<ItemHealth>();
        h.ResetHealth();
    }
    [Client] private void Respawn() { //called by local player to respawn.
        if(!isLocalPlayer) return;
        RespawnGen();
        CMDPlayerRespawn();
    }

    [Command] private void CMDPlayerRespawn() { //updates attacker sprite on server
        RespawnGen();
        RPCPlayerRespawn();
    }

    [ClientRpc] private void RPCPlayerRespawn() { //updates attacker sprite on clients 
        if(isLocalPlayer) {
            return;
        }
        RespawnGen();
    }
    //******************RESPAWN******************\\


    void OnCollisionStay2D(Collision2D other) {
        if(!LevelManager.instance.isSpawning) { //if not in round dont do damage
            return;
        }
        GameObject go=other.gameObject;
        if(go.layer==8 && LevelManager.instance.isSpawning) {
            ItemHealth H=go.GetComponent<ItemHealth>();
            if(timeSenseLastAttack>=timeToAttack) {
                Debug.Log("Attack");
                H.TakeDamage(damage);
                timeSenseLastAttack=0;
            }
            timeSenseLastAttack+=Time.deltaTime;
        }
    }

    public void ScreenMouseRay() { //May want to return the transform to make more generic
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 5f;
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Collider2D[] col = Physics2D.OverlapPointAll(worldPosition);
        if(col.Length > 0){
            foreach(Collider2D c in col) {
                if(c.gameObject.tag=="Respawn") {
                    home=c.gameObject.transform;
                    Debug.Log("Home set");
                }
            }
        }
    }

    public void Movement() {
        float xDirection=Input.GetAxis("Horizontal"); 
        float yDirection= Input.GetAxis("Vertical");
        Vector3 moveDirection =new Vector3(xDirection, yDirection, 0.0f);
        transform.position += moveDirection * moveSpeed;
    }
}
