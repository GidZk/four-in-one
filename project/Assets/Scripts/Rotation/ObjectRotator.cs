        using UnityEngine;
    //TODO refine this code by extending this ojbect and move functionality
        public class ObjectRotator : MonoBehaviour
        {
            /*object to be rotated*/
            public GameObject target;

            public bool isTerminating;
            public bool isTouch;
            public int rotationSpeedLimit;
            public int  maxRotations;
            public float radius;

            private Transform pivot;
            private RotationCalculator calc;
            private bool isMouseDown;

            void Start()
            {

                radius = 1;
                pivot = target.transform;
                transform.parent = pivot;
                transform.position += Vector3.up * radius;
                isMouseDown = false;
                calc = new RotationCalculator( maxRotations);
            }

            void Update()
            {
                if (isMouseDown || Input.touchCount > 0){
                    if (isFinished(calc.accumulatedAngle,  maxRotations) && isTerminating) {
                        OnReset();
                    }
                    // not sure which camera to use
                    Vector3 objectToMouse = Camera.main.WorldToScreenPoint(target.transform.position) - Input.mousePosition;
                    calc.CalculateRotationAngle(objectToMouse);
                    
                    if (isTerminating) {
                        calc.CalcAndSetFields(objectToMouse, rotationSpeedLimit);
                    }

                    pivot.position = target.transform.position;
                    pivot.rotation = Quaternion.AngleAxis(calc.theta , Vector3.forward);
                }
            }

            private void OnReset()
            {
                calc.Reset();
                //TODO call other classes to tell roation complete
            }



            private bool isFinished(float accumulatedTheta, int terminatingRotations) {
               return  (Mathf.Floor(accumulatedTheta / 360f) > terminatingRotations);
            }


            // to be able to debug with mouse
            private void OnMouseDown()
                {
                    Vector3 objectToMouse = Camera.main.WorldToScreenPoint(target.transform.position) - Input.mousePosition;
                    calc.prevTheta = (Mathf.Atan2((objectToMouse.y), objectToMouse.x) * Mathf.Rad2Deg) + 180;
                    isMouseDown = true;

                }


            private void OnMouseUp()
            {
                isMouseDown = false;
            }



            private void DebugLogValues()
            {
                Debug.Log("ObjectRotator:: Theta: " + (calc.theta));
                Debug.Log("ObjectRotator::      :" + calc.GetRotations());
                Debug.Log("ObjectRotator:: Ratio :" + calc.ratio);
            }

        }
