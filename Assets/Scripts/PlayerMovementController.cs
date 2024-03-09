using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMovementController : NetworkBehaviour
{
    public GameObject PlayerModel;
    public Rigidbody2D rb;
    private bool changed=false;
    private PlayerObjectController parent;
    public DefenderRuleset rulesD;
    public AttackerRuleSet rulesA;

    private CustomNetworkManager manager;
    private CustomNetworkManager Manager {
        get {
            if(manager!=null) {
                return manager;
            } else {
                return manager=CustomNetworkManager.singleton as CustomNetworkManager; 
            }
        }
    }

    private void Start() {
        PlayerModel.SetActive(true);
        SetPosition();
        parent=this.GetComponentInParent<PlayerObjectController>();
    }

    private void Update() {
        if(SceneManager.GetActiveScene().name=="GameBoard1") { //find more effecient way to detect scene
            if(isOwned) {
                //**********Setup**********
                if(!changed) {//only happens at start
                    // if(!NetworkClient.ready) {
                    //     Debug.Log("Not ready");
                    //     return;
                    // }          
                    if (!NetworkClient.ready) NetworkClient.Ready();
                    if(GameObject.FindWithTag("DefenderUI")) {
                        GameObject.FindWithTag("DefenderUI").SetActive(parent.isDefender);
                    }
                    if(GameObject.FindWithTag("AttackerUI")) {
                        GameObject.FindWithTag("AttackerUI").SetActive(!parent.isDefender);
                    }
                    //activate ruleSet for each player
                    if(parent.isDefender) {
                        rulesD=GetComponentInParent<DefenderRuleset>();
                        rulesD.Inst();
                        Destroy(this.GetComponentInParent<AttackerRuleSet>()); //what happens if we play another game?
                    } else {
                        //Attacker rules
                        rulesA=GetComponentInParent<AttackerRuleSet>();
                        rulesA.Inst();
                        Destroy(this.GetComponentInParent<DefenderRuleset>());
                    }
                    changed=true;
                }
                //**********Setup**********
                // Movement(); //Need to move to specific attacker/defender scripts
                if(parent.isDefender) {
                    DefenderStuff();
                } else {
                    AttackerStuff();
                }
            }        
        }
    }

    //Attacker RuleSet
    public void AttackerStuff() {
        rulesA.Actions();
    }

    //Defender RuleSet
    public void DefenderStuff() {
        rulesD.Actions();
    }

    public void SetPosition() {
        transform.position=new Vector3(-49f, 20.5f, 0.0f);
    }
}
