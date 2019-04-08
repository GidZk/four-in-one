using UnityEngine;

public class MouseRotator : MonoBehaviour
{

    /*object to be rotated*/
    public GameObject target;
    /*inserted radius in case we want to rotate something around another thing*/
    public float radius;
    public float speedLimit;

    private Transform pivot;
    private bool isMouseDown;
    private float prevTheta;
    private float theta;
    private float dTheta;

    private float accTheta;

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
            calculateRotParams();
            pivot.position = target.transform.position;
            pivot.rotation = Quaternion.AngleAxis(theta , Vector3.forward);
        }

    }




    private void calculateRotParams() {
        Vector3 objectToMouse = Camera.main.WorldToScreenPoint(target.transform.position) - Input.mousePosition;
        if (Input.GetTouch(0).phase == TouchPhase.Began){
            prevTheta = (Mathf.Atan2((objectToMouse.y), objectToMouse.x) * Mathf.Rad2Deg) + 180;
        }

        theta = (Mathf.Atan2((objectToMouse.y), objectToMouse.x) * Mathf.Rad2Deg) + 180;

        if (Mathf.Abs(theta - prevTheta) > speedLimit){
            dTheta = speedLimit;
        }
        else{
            dTheta = theta - prevTheta;
        }

        prevTheta = theta;
        accTheta += Mathf.Abs(dTheta);

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
}
