using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEditor;

public class AttackerRuleSet : NetworkBehaviour
{
    [SerializeField] int damage=20; //may not need, maybe apply to weapon
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private GameObject spottingSystemPrefab;
    [SyncVar] private Color col=new Color(1,0,0,1);
    private float timeToAttack=1.5f;
    private float timeSenseLastAttack=1.5f;
    public bool isDead=false;
    private bool changed=false;
    public float moveSpeed= 0.1f;
    public Transform home; //default
    private SpriteMask mask; 
    public float timeSenseGetFollower=15f;
    private GetFollowerScript minionGrabber;
    private SpikeManager spikeManager;
    [SerializeField] GameObject spikeManagerAbility;

    public void Inst() {
        CLFogOfWarGen();
        LevelManager.instance.SetFOW();
        minionGrabber=gameObject.AddComponent<GetFollowerScript>();
        spikeManager=spikeManagerAbility.GetComponent<SpikeManager>();
        spikeManager.Inst();
        sr.color=new Color(1f, 0.0f, 0.0f, 1);//only local player red.
    }

    public void Actions() {
        Movement();
        //***********TIMERS***********\\
        timeSenseGetFollower+=Time.deltaTime;    
        //***********TIMERS***********\\
        if(!LevelManager.instance.isSpawning) { //if not in a round
            if(LevelManager.instance.timeBetweenWaves-LevelManager.instance.lastWaveTime<=1.5f) { //about to start round
                if(!home) {
                    home=LevelManager.instance.spawners[0].transform;    
                }
                gameObject.transform.position=home.position;
                Respawn();
            }
            if(Input.GetMouseButton(0)) {
                HomeCheck(ScreenMouseRay());
            }
        } 
        
        else { //if in a round
            if(isDead && !changed) {
                PlayerDead();
            } 
            else { //if in a round
                if(LevelManager.instance.roundTime<=.5f) {
                    if(!home) {
                        home=LevelManager.instance.spawners[0].transform;
                        minionGrabber.SetFree();    
                    }
                    gameObject.transform.position=home.position;
                    Respawn();
                }
                if(Input.GetKey(KeyCode.Alpha1) && timeSenseGetFollower>=minionGrabber.coolDownTime && !Input.GetKey(KeyCode.LeftShift) && minionGrabber.minionCount<minionGrabber.minionLimit) {
                    timeSenseGetFollower=0f;
                    minionGrabber.GrabFollowers();
                    }
                if(Input.GetKey(KeyCode.Alpha1) && Input.GetKey(KeyCode.LeftShift)) {
                    minionGrabber.SetFree();
                }
                // if(Input.GetKey(KeyCode.Alpha2)) { //spike
                //     bool canBuild=true;
                //     Collider2D[] col=ScreenMouseRay();
                //     if(col.Length > 0) {
                //         foreach(Collider2D c in col) {
                //             if(c.gameObject.layer==4 || c.gameObject.layer==8) {
                //                 canBuild=false;
                //             }
                //         }
                //         if(canBuild) {
                //             spikeManager.AddSpike(gameObject.transform);
                //         }
                //     }
                // }
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
        mask=gameObject.GetComponentInChildren<SpriteMask>();
        mask.frontSortingOrder=0;
        GameObject maskA=Instantiate(spottingSystemPrefab);
        maskA.transform.parent=gameObject.transform;
        maskA.transform.position=gameObject.transform.position;
        gameObject.transform.position=new Vector3(Random.Range(-5, 5), 10, 0);
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
        col=new Color(0, 0.0f, 0.0f, 1);
        sr.color= col;
        ItemHealth h =GetComponentInParent<ItemHealth>();
        h.ResetHealth();
        h.ResetKilled();
        // minionGrabber.SetFree();
        // minionGrabber.LevelUp();
    }
    [Client] private void Respawn() { //called by local player to respawn.
        if(!isLocalPlayer) return;
        RespawnGen();
        CMDPlayerRespawn();
        sr.color=new Color(1,0,0,1);
        if(spikeManager) {
            spikeManager.CheckSpikes();            
        }
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
        if(!LevelManager.instance) {
            return;
        }
        if(!LevelManager.instance.isSpawning) { //if not in round dont do damage
            return;
        }
        GameObject go=other.gameObject;
        if(go.tag=="Citadel") {
            return;
        }
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

    private Collider2D[] ScreenMouseRay() {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 5f;
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        return Physics2D.OverlapPointAll(worldPosition);
    }

    private void HomeCheck(Collider2D[] col) {
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
