using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMovementController : NetworkBehaviour
{
    public float moveSpeed= 0.1f;
    public GameObject PlayerModel;
    public Rigidbody2D rb;
    //public GameObject selectedSpawner;
    private bool changed=false;

    private void Start() {
        PlayerModel.SetActive(true);
        SetPosition();
    }

    private void FixedUpdate() {
        if(SceneManager.GetActiveScene().name=="GameBoard1") {
            if(!changed && isOwned) {
                PlayerObjectController thing=this.GetComponentInParent<PlayerObjectController>();
                GameObject.Find("DefenderUI").SetActive(thing.isDefender);
                GameObject.Find("AttackerUI").SetActive(!thing.isDefender);
                Debug.Log("SetUI "+thing.isDefender.ToString());
                changed=true;
            }
            if(isOwned) {
                Movement();
            }
        }
    }

    public void Movement() {
        float xDirection=Input.GetAxis("Horizontal"); 
        float yDirection= Input.GetAxis("Vertical");

        UnityEngine.Vector3 moveDirection =new Vector3(xDirection, yDirection, 0.0f);

        transform.position += moveDirection * moveSpeed;
    }

    //Attacker RuleSet
    public void Attack() {

    }

    //Defender RuleSet
    public void DefenderStuff() {

    }

    public void SetPosition() {
        //transform.position=selectedSpawner.transform.position;
        transform.position=new Vector3(-30.5f, 30.5f, 0.0f);
    }
}
