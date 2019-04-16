using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class playerController : MonoBehaviour, InputListener
{
    private Rigidbody2D rb;

    private Vector2 moveInput;

    //private Vector2 moveVelocity;
    private float hInput = 0;
    private float vInput = 0;

    [SerializeField] private float _hInput;
    [SerializeField] private float _vInput;

    public const float SpeedFactorConstant = 45f;

    public static playerController Instance { get; private set; }


    // Start is called before the first frame update


    private void Awake()
    {
        if (Instance != null)
        {
            throw new Exception("Instance is not null ");
        }

        Instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        NetworkController.Instance.Register(this);
    }


    void FixedUpdate()
    {
        //player inside of boundary
        InsideGUI();

        if (Math.Abs(_hInput) < 0.1f)
        {
        } // decelerate maybe
        else
        {
            rb.AddForce(new Vector2(_hInput, 0) * SpeedFactorConstant);
        }

        if (Math.Abs(_vInput) < 0.1f)
        {
        } // decelerate maybe
        else
        {
            rb.AddForce(new Vector2(0, _vInput) * SpeedFactorConstant);
        }
    }
  

    // this method will be called by the client that has the server locally,
    // after a remote client has commanded the client which has the server to do so.
    public void OnHorizontalMovementInput(float value)
    {
        _hInput = value;
    }

    public void OnVerticalMovementInput(float value)
    {
        _vInput = value;
    }

    public void LinearDrag(float value)
    {
        rb.drag = value;
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
        if (coll.gameObject.CompareTag("crabplast"))
        {
            Debug.Log($"{this} --a collision between player and crab. ");

            Destroy(coll.gameObject);

            //Add 1 point each time the starfish(object that gives points) collides

            // with the object this script is attached to
            AddScore.scoreValue++;

            //add time to the timebar when catching crab
            Timer.timeLeft++;
            Debug.Log("score = " + AddScore.scoreValue);
        }
    }


    private void InsideGUI()
    {
        // if (BoundaryController.  object.objectcollide Equals(true))
        if (transform.position.y > 28f)
        {

            transform.position = new Vector3(transform.position.x, 28f, transform.position.z);
            Debug.Log("outside background");
        }
        if (transform.position.y < -28f)
        {
            transform.position = new Vector3(transform.position.x, -28f, transform.position.z);
        }
        if (transform.position.x < -38f)
        {
            transform.position = new Vector3(-38f, transform.position.y, transform.position.z);
        }
        if (transform.position.x > 38f)
        {
            transform.position = new Vector3(38f, transform.position.y, transform.position.z);
        }

    }
}