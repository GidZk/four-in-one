using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour {

    public float moveSpeed;
    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 moveVelocity;
    private float hInput = 0;
    private float vInput = 0;


    // Start is called before the first frame update
    void Start(){
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update(){
        if(Input.touchCount > 0){
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began){
                Move(hInput, vInput);
            }
            else if(touch.phase == TouchPhase.Ended){
                Debug.Log(hInput);
                Move(0,0);
            }
                
        }
        //moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        //Move(hInput);
    }        

    void FixedUpdate (){
        
        //uncomment for arrows instead
        //Move(Input.GetAxisRaw("Horizontal"));
        
    }
    
    public void Move ( float horizontalInput, float verticalInput){

        Vector2 moveVelocity = rb.velocity;
        moveVelocity.x = horizontalInput * moveSpeed;
        moveVelocity.y  = verticalInput * moveSpeed;
        rb.velocity = moveVelocity;
        /* 
        if(Input.touchCount > 0){
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Ended){
                Debug.Log(hInput);
                moveVelocity.x = 0;
                moveVelocity.y = 0;
                rb.velocity = moveVelocity;

            }
        }*/    

    } 

    public void HorizontalMovement(float horizontalInput){
        hInput = horizontalInput;
        
    }
    public void VerticalMovement(float VerticalInput){
        vInput = VerticalInput;
    }
}
