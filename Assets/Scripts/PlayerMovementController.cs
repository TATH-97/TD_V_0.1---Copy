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
    Vector2 movement;

//     void Start() {
//         PlayerModel.SetActive(false);
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         movement.y=Input.GetAxisRaw("Vertical");
//         movement.x=Input.GetAxisRaw("Horizontal");
//     }

//     void FixedUpdate() {
//         rb.MovePosition(rb.position+movement*moveSpeed*Time.fixedDeltaTime);        
//     }

//     void OnCollisionEnter(Collision col) {
//         Debug.Log("OW!");
//     }

//     public void Movement() {
//         float xDirection=Input.GetAxis("Horizontal"); 
//         float yDirection= Input.GetAxis("Vertical");

//         Vector2 moveDirection =new Vector2(xDirection, yDirection);

//         transform.position = (moveDirection * moveSpeed) + transform.position;
//     }
}
