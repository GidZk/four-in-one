    using UnityEngine;

    public class ObjectRotator : MonoBehaviour
    {
        /*object to be rotated*/
        public GameObject target;
        /*inserted radius in case we want to rotate something around another thing*/
        public float radius;
        public float speedLimit;
        public bool isTerminating;
        public bool isTouch;
        public int nOfRotations;

        private Transform pivot;
        private bool isMouseDown;
        private float prevTheta;
        private float accumulatedAngle;
        private float finishedRatio { get; set; }


        void Start()
        {
            radius = 1;
            pivot = target.transform;
            transform.parent = pivot;
            transform.position += Vector3.up * radius;
            isMouseDown = false;
            finishedRatio = 0;
        }

        void Update()
        {
            if (isMouseDown || Input.touchCount > 0){
                if (isFinished(accumulatedAngle, nOfRotations) && isTerminating) {
                    OnReset();
                }
                // not sure which camera to use
                Vector3 objectToMouse = Camera.main.WorldToScreenPoint(target.transform.position) - Input.mousePosition;
                float theta = (Mathf.Atan2((objectToMouse.y), objectToMouse.x) * Mathf.Rad2Deg) + 180;
                if (isTerminating) {
                accumulatedAngle += Mathf.Abs(CalculateDeltaTheta(objectToMouse, theta));
                finishedRatio = accumulatedAngle / (nOfRotations * 360);
                prevTheta = theta;

                }

                pivot.position = target.transform.position;
                pivot.rotation = Quaternion.AngleAxis(theta , Vector3.forward);
                //LogValues(theta, CalculateDeltaTheta(objectToMouse, theta), accumulatedAngle, finishedRatio);

            }

        }

        private float CalculateDeltaTheta(Vector3 objectToMouse, float theta) {

            if (isTouch && Input.GetTouch(0).phase == TouchPhase.Began) {
            prevTheta = (Mathf.Atan2((objectToMouse.y), objectToMouse.x) * Mathf.Rad2Deg) + 180;
            }

            if (Mathf.Abs(theta - prevTheta) > speedLimit){
                return speedLimit;
            }
               return (theta - prevTheta);
        }

        private void OnReset()
        {
            accumulatedAngle = 0;
            //TODO call other classes to tell roation complete
        }



        private bool isFinished(float accumulatedTheta, int terminatingRotations) {
           return  (Mathf.Floor(accumulatedTheta / 360f) >= terminatingRotations);
        }


        private void LogValues(float theta, float dTheta, float accumulatedAngle, float ratio) {
            //Debug.Log("ObjectRotator:: Theta: " + (theta));
            //Debug.Log("ObjectRotator:: Delta Theta: " + dTheta);
            Debug.Log("ObjectRotator:: Rotations :" + (Mathf.Floor(accumulatedAngle / 360f)));
            Debug.Log("ObjectRotator:: Ratio :" + ratio );
        }



    public virtual void Init() { 


    }

    // to be able to debug with mouse
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
