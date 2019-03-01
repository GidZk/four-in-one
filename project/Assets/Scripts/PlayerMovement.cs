using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    //public Rigidbody rb;
    public float runSpeed = 40f;
    float horizontalMove = 0f;
    float verticalMove = 0f;
    public Vector2 touchOrigin = -Vector2.one;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      #if UNITY_STANDALONE || UNITY_WEBPLAYER // to enable both type of controllers

        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        verticalMove = Input.GetAxisRaw("Vertical") * runSpeed;
      #else
      if(Input.touchCount > 0){
        Touch myTouch = Input.touches[0];

        if(myTouch.phase == TouchPhase.Began){
          touchOrigin = myTouch.position;
        }
        else if(myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0){
          Vector2 touchEnd = myTouch.position;
          float x = touchEnd.x - touchOrigin.x;
          touchOrigin.x = -1;
          
            horizontalMove = x > 0 ? 1 : -1;
      
        }
        #endif
      }        

    }

    void FixedUpdate() //For physics
    {
  controller.Move(horizontalMove * Time.fixedDeltaTime, false, false);
  //      rb.Move(horizontalMove * Time.fixedDeltaTime, false, false);
    }

}
