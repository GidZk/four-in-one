using UnityEngine;

public class MouseRotator : MonoBehaviour
{
    /*object to be rotated*/
    public GameObject obj;
    /*inserted radius in case we want to rotate something around another thing*/
    public bool isTouch;
    private float radius;
    private Transform pivot;
    private bool isMouseDown;

    private float prevTheta;
    private float theta;
    private float dTheta;

    private float accTheta;

    void Start()
    {
        prevTheta = 0;
        radius = 1;
        pivot = obj.transform;
        transform.parent = pivot;
        transform.position += Vector3.up * radius;
        isMouseDown = false;



    }

    void Update()
    {
        if (isMouseDown)
        {


            Vector3 objectToMouse = Camera.main.WorldToScreenPoint(obj.transform.position) - Input.mousePosition;

             //  double theta = Mathf.Atan(orbVector.x / orbVector.y) * (180 / Mathf.PI);


      

            theta = (Mathf.Atan2((objectToMouse.y), objectToMouse.x)* Mathf.Rad2Deg) + 180;

            dTheta = theta - prevTheta;
            prevTheta = theta;
            accTheta += dTheta;




            Debug.Log("Rotations:: Theta: " + (theta));
            //Debug.Log("MouseRotator:: Delta Theta: " + dTheta);
            //Debug.Log("MouseRotator:: AccTheta :" + accTheta);
            //Debug.Log("MouseRotator:: Rotations :" +(accTheta / 360f));
            pivot.position = obj.transform.position;
            pivot.rotation = Quaternion.AngleAxis(theta - 90, Vector3.forward);
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
