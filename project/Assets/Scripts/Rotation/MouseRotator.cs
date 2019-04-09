using UnityEngine;

public class MouseRotator : MonoBehaviour
{

    /*object to be rotated*/
    public GameObject target;
    /*inserted radius in case we want to rotate something around another thing*/
    public float radius;
    public float speedLimit;
    public bool isTerminating;
    public bool isTouch;
    public int nRotationsToFinish;

    private Transform pivot;
    private bool isMouseDown;
    private float prevTheta;
    private float accumulatedAngle;
   

    void Start()
    {
        radius = 1;
        pivot = target.transform;
        transform.parent = pivot;
        transform.position += Vector3.up * radius;
        isMouseDown = false;

    }

    void Update()
    {
        if (isMouseDown || Input.touchCount > 0)
        {
            if (isFinished(accumulatedAngle, nRotationsToFinish) && isTerminating) {
                //TODO: reset fields, call trigger event etc.
            }

            Vector3 objectToMouse = Camera.main.WorldToScreenPoint(target.transform.position) - Input.mousePosition;
            float theta = (Mathf.Atan2((objectToMouse.y), objectToMouse.x) * Mathf.Rad2Deg) + 180;
            accumulatedAngle += Mathf.Abs(CalculateDeltaTheta(objectToMouse, theta));
            prevTheta = theta;
            

            pivot.position = target.transform.position;
            pivot.rotation = Quaternion.AngleAxis(theta , Vector3.forward);

            logValues(theta, CalculateDeltaTheta(objectToMouse, theta), accumulatedAngle);




        }

    }




    private float CalculateDeltaTheta(Vector3 objectToMouse, float theta) {
        float dTheta; 

        if (isTouch && Input.GetTouch(0).phase == TouchPhase.Began)
            prevTheta = (Mathf.Atan2((objectToMouse.y), objectToMouse.x) * Mathf.Rad2Deg) + 180;


        if (Mathf.Abs(theta - prevTheta) > speedLimit){
            dTheta = speedLimit;
        }
        else{
            dTheta = theta - prevTheta;
        }

        return dTheta;



    }

    private void OnMouseDown()
    {
        Vector3 objectToMouse = Camera.main.WorldToScreenPoint(target.transform.position) - Input.mousePosition;
        prevTheta = (Mathf.Atan2((objectToMouse.y), objectToMouse.x) * Mathf.Rad2Deg) + 180;
        isMouseDown = true;

    }

    private void OnMouseUp()
    {
        isMouseDown = false;
    }


    private bool isFinished(float accumulatedTheta, int terminatingRotations) {
       return  (Mathf.Floor(accumulatedTheta / 360f) >= terminatingRotations);
    }


    private void logValues(float theta, float dTheta, float accumulatedAngle) {
        //Debug.Log("WheelRotator:: Theta: " + (theta));
        //Debug.Log("WheelRotator:: Delta Theta: " + dTheta);
        Debug.Log("WheelRotator:: Rotations :" + (Mathf.Floor(accumulatedAngle / 360f)));

    }

}
