using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyEventTrigger : EventTrigger
{
    NetworkController nwController;
    private bool isClickedYaoo;

    private void Awake()
    {
        nwController = NetworkController.Instance;
    }
    // Start is called before the first frame update
    void Start()
    {

               
    }


    private void FixedUpdate()
    {
        if (isClickedYaoo)
            nwController.OnHorizontalMovementInput(1);

    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);  
        isClickedYaoo = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        isClickedYaoo = false;
    }





    public override void OnPointerClick(PointerEventData eventData)
    {
            base.OnPointerClick(eventData);
        Debug.Log("is network controller null ?????" + nwController );
        nwController.OnHorizontalMovementInput(1);
            }

    void MoveRight()
    {
        nwController.OnHorizontalMovementInput(1);

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
