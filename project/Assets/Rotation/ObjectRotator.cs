﻿using UnityEngine;
using UnityEngine.Networking;
//TODO refine this code by extending this ojbect and move functionality
public class ObjectRotator : NetworkBehaviour
{

    //public GameObject centerObject;

    public bool isTerminating;
    public bool isTouch;
    public int rotationSpeedLimit;
    public int maxRotations;
    

    private float theta { get; set; }
    private float prevTheta { get; set; }
    private float accumulatedAngle { get; set; }
    private float ratio { get; set; }

  

    private bool isMouseDown;

    void Start()
    {
        transform.position += Vector3.up;
        isMouseDown = false;
    
    }

    void Update()
    {

        if (isMouseDown)
        {
        //    NetworkClient.Send(Messages.Control,new ControlMessage(Messages.Control,ControlType.CannonAngle));


        }


        if (isMouseDown || Input.touchCount > 0)
        {
            if (isFinished(accumulatedAngle, maxRotations) && isTerminating)
            {
                OnReset();
            }
            // not sure which camera to use
            Vector3 objectToMouse = Camera.main.WorldToScreenPoint(transform.position) - Input.mousePosition;
            theta = CalculateRotationAngle(objectToMouse);

            if (isTerminating)
            {
                accumulatedAngle += Mathf.Abs(CalculateDeltaTheta(objectToMouse, theta, rotationSpeedLimit));
                ratio = accumulatedAngle / (maxRotations * 360);
            }

            transform.position = transform.position;
            transform.Rotate(Vector3.forward, (theta - prevTheta));

            prevTheta = theta;
        }
    }

    private void OnMouseDown()
    {
        Vector3 objectToMouse = Camera.main.WorldToScreenPoint(transform.position) - Input.mousePosition;
        prevTheta = (Mathf.Atan2((objectToMouse.y), objectToMouse.x) * Mathf.Rad2Deg) + 180;
        isMouseDown = true;
    }


    private void OnMouseUp()
    {
        isMouseDown = false;
    }

    public Vector3 GetEulerAngles()
    {
        return transform.eulerAngles;
    }


    // --------- private methods -------------


    private float CalculateDeltaTheta(Vector3 objectToMouse, float theta, int speedLimit)
    {
        if (Mathf.Abs(theta - prevTheta) > speedLimit)
        {
            return speedLimit;
        }
        return (theta - prevTheta);
    }


    private float CalculateRotationAngle(Vector3 objectToMouse)
    {
        return (Mathf.Atan2((objectToMouse.y), objectToMouse.x) * Mathf.Rad2Deg) + 180;
    }

    private void OnReset()
    {
        accumulatedAngle = 0;
        //TODO call other classes to tell roation complete
    }

    private bool isFinished(float accumulatedTheta, int terminatingRotations)
    {
        return (Mathf.Floor(accumulatedTheta / 360f) >= terminatingRotations);
    }


    private float GetNOfRotations()
    {
        return Mathf.Floor(accumulatedAngle / 360);
    }


    // to be able to debug with mouse



    private void DebugLogValues()
    {
        //Debug.Log("ObjectRotator:: Theta: " + (theta));
        Debug.Log("ObjectRotator::      :" + GetNOfRotations());
        Debug.Log("ObjectRotator:: Ratio :" + ratio);
    }

}