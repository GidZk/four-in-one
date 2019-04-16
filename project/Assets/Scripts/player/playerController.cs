using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class playerController : MonoBehaviour, InputListener
{
    public float moveForce;
    private Rigidbody2D rb;

    private Vector2 moveInput;

    //private Vector2 moveVelocity;
    private float hInput = 0;
    private float vInput = 0;

    private NetworkController nwController;


    // Start is called before the first frame update

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        nwController = NetworkController.Instance;
        nwController.Register(this);
    }


    // ============ old code to be removed / updated for touches ================== 
    /*
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
    // ==============================================================================
    */
    // ============ old code to be removed / updated for touches ================== // 
    void FixedUpdate()
    {
        // delegate movement to server in msg passing system
        if (Input.GetKey(KeyCode.UpArrow))
            nwController.OnVerticalMovementInput(moveForce);
        if (Input.GetKey(KeyCode.DownArrow))
            nwController.OnVerticalMovementInput(-moveForce);
        if (Input.GetKey(KeyCode.RightArrow))
            nwController.OnHorizontalMovementInput(moveForce);
        if (Input.GetKey(KeyCode.LeftArrow))
            nwController.OnHorizontalMovementInput(-moveForce);
    }


    // this method will be called by the client that has the server locally,
    // after a remote client has commanded the client which has the server to do so.
    public void OnHorizontalMovementInput(float value)
    {
        rb.AddForce(new Vector2(value, 0));
    }

    public void OnVerticalMovementInput(float value)
    {
        rb.AddForce(new Vector2(0, value));
    }


    public void OnCannonAngleInput(float value)
    {
    }

    public void OnCannonLaunchInput(float value)
    {
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
}