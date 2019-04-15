using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyEventTrigger : EventTrigger

{
    private playerController _playerController;
    
    private bool isClickedYaoo;
    private bool isUp, isDown;
    private bool isRight, isLeft;

    private float moveForce;
   
    private void Awake()
    {
        _playerController = playerController.instance;
        isClickedYaoo = false;
        InitIsRight();
        InitIsUp();
        InitIsDown();
        InitIsLeft();

        moveForce = GetMoveForce();

    }
    

    
    

 


    private void FixedUpdate()
    {


        if (isClickedYaoo)
        {
            
            if (!isRight &&!isLeft && !isUp )
            { Debug.Log("down called");
                _playerController.OnVerticalMovementInput(5);
            }
            //right
            if (isRight &&!isLeft && !isUp  )
            {
                Debug.Log("right called");
                _playerController.OnHorizontalMovementInput(moveForce);
                
            }
            if (!isRight && isLeft && !isUp)
            {
                Debug.Log("left called");
                _playerController.OnHorizontalMovementInput(- moveForce);
            }    

            if (!isRight && isUp && !isDown)
            { Debug.Log("up called");
                _playerController.OnVerticalMovementInput(moveForce);
            }



            
        }



    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);  
       
        
        isClickedYaoo = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        isClickedYaoo = false;
    }

   private float  GetMoveForce()
   {
       return GameObject.FindWithTag("Player").GetComponent<playerController>().moveForce;
   }


    private void InitIsRight()
    {
        isRight = (gameObject.tag == "RightButton");
    }
    
    private void InitIsLeft()
    {
        isLeft = (gameObject.tag == "LeftButton");
    }


    private void InitIsUp()
    {
        isUp = (gameObject.tag == "UpButton");
    }
    
    private void InitIsDown()
    {
        isDown = (gameObject.tag == "DownButton");
    }
    
}
