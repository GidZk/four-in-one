using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRotator : ObjectRotator 
{
    /*object to be rotated*/
    public GameObject obj;

    /*inserted radius in case we want to rotate something around another thing*/
    private float radius;
    private Transform pivot;
    private bool isMouseDown;
    

    void Start()
    {
        radius = 1;
        pivot = obj.transform;
        transform.parent = pivot;
        transform.position += Vector3.up * radius;
        isMouseDown = false;
    }

    void Update()
    {
        if (isMouseDown) {
            Vector3 orbVector = Camera.main.WorldToScreenPoint(obj.transform.position);
            orbVector = Input.mousePosition - orbVector;
            float angle = Mathf.Atan2(orbVector.y, orbVector.x) * Mathf.Rad2Deg;

            pivot.position = obj.transform.position;
            pivot.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }

    }


    private void OnMouseDown()
    {
        isMouseDown = true;

    }

    private void OnMouseUp()
    {

        isMouseDown = false;

    }

}
