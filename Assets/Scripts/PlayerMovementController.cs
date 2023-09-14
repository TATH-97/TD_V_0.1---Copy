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

    private void Start() {
        PlayerModel.SetActive(true);
    }

    private void FixedUpdate() {
        if(SceneManager.GetActiveScene().name=="GameBoard1") {
            if(PlayerModel.activeSelf==false) {
                SetPosition();
                PlayerModel.SetActive(true);
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

    public void SetPosition() {
        //transform.position=selectedSpawner.transform.position;
        transform.position=new Vector3(-10.5f, 10.5f, 0.0f);
    }
}
