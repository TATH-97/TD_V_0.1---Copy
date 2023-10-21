using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Unity.Mathematics;

public class PlayerMovementController : NetworkBehaviour
{
    public float moveSpeed= 0.1f;
    public GameObject PlayerModel;
    public Rigidbody2D rb;
    //public GameObject selectedSpawner;
    private bool changed=false;
    private PlayerObjectController parent;
    public DefenderRuleset rulesD;
    public AttackerRuleSet rulesA;

    private void Start() {
        PlayerModel.SetActive(true);
        SetPosition();
        parent=this.GetComponentInParent<PlayerObjectController>();
    }

    private void FixedUpdate() {
        if(SceneManager.GetActiveScene().name=="GameBoard1") { //find more effecient way to detect scene
            if(isOwned) {
                //**********Setup**********
                if(!changed) {//only happens at start
                    GameObject.Find("DefenderUI").SetActive(parent.isDefender);
                    GameObject.Find("AttackerUI").SetActive(!parent.isDefender);
                    //activate ruleSet for each player
                    if(parent.isDefender) {
                        rulesD=GetComponentInParent<DefenderRuleset>();
                        rulesD.Inst();
                        Destroy(this.GetComponentInParent<AttackerRuleSet>());
                    } else {
                        //Attacker rules
                        rulesA=GetComponentInParent<AttackerRuleSet>();
                        Destroy(this.GetComponentInParent<DefenderRuleset>());
                    }
                    changed=true;
                }
                //**********Setup**********
                Movement(); //Need to move to specific attacker/defender scripts

                if(parent.isDefender) {
                    DefenderStuff();
                } else {
                    AttackerStuff();
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

    //Attacker RuleSet
    public void AttackerStuff() {
        
    }

    //Defender RuleSet
    public void DefenderStuff() {
        if(Input.GetMouseButton(0)) {
            rulesD.ScreenMouseRay();
        }
    }

    public void ArmClicker() {
        Debug.Log("thing");
    }

    public void SetPosition() {
        //transform.position=selectedSpawner.transform.position;
        transform.position=new Vector3(-30.5f, 30.5f, 0.0f);
    }
}
