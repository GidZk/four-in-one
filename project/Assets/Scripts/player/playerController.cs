using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class playerController : MonoBehaviour, InputListener {

    
    public float moveSpeed;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 moveVelocity;
    private float hInput = 0;
    private float vInput = 0;

    private NetworkController nwController;


    // Start is called before the first frame update

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        nwController = NetworkAssets.GetController();
        nwController.Register(this);

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

    void FixedUpdate()
    {
        //uncomment for arrows instead
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        if(horizontal > 0 )
           nwController.OnHorizontalMovementInput(horizontal);


     
        //Move(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
    
    
    
    
    
    public void Move ( float horizontalInput, float verticalInput){

        Vector2 moveVelocity = rb.velocity;
        moveVelocity.x = horizontalInput * moveSpeed;
        moveVelocity.y  = verticalInput * moveSpeed;

        rb.velocity = moveVelocity;
        //Debug.Log(rb.velocity);
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


    

    // Invoked on collision
  void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "crabplast")
        {
            Debug.Log($"{this} --a collision between player and alga. ");

            Destroy(coll.gameObject);

            //Add 1 point each time the starfish(object that gives points) collides

            // with the object this script is attached to
            AddScore.scoreValue++;

            //add time to the timebar when catching crab
            Timer.timeLeft++;
            Debug.Log("score = " + AddScore.scoreValue);
        }
    }
  
  
  
  

  public void OnVerticalMovementInput(float value)
  {
      Move(value,0);
  }

  public void OnHorizontalMovementInput(float value)
  {
      throw new System.NotImplementedException();
  }

  public void OnCannonAngleInput(float value)
  {
      throw new System.NotImplementedException();
  }

  public void OnCannonLaunchInput(float value)
  {
      throw new System.NotImplementedException();
  }
}
