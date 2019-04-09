    using System;
    using UnityEngine;

    public class RotationCalculator {

        public float theta { get; set; }
        public float prevTheta { get; set; }
        public float accumulatedAngle {get; set;}
        public float ratio { get; set; }
        private int maxRotations;


        public RotationCalculator(int nOfRotations){
            ratio = 0;
            maxRotations = nOfRotations;
        }

        public void CalculateRotationAngle(Vector3 objectToMouse) {
            theta = (Mathf.Atan2((objectToMouse.y), objectToMouse.x) * Mathf.Rad2Deg) + 180;
    }



    public void CalcAndSetFields(Vector3 objectToMouse , int limit) {
            accumulatedAngle += Mathf.Abs(CalculateDeltaTheta(objectToMouse, theta, limit));
            ratio = accumulatedAngle / (maxRotations * 360);
            prevTheta = theta;
        }

        public void Reset() {
            accumulatedAngle = 0;
        }

        private float CalculateDeltaTheta(Vector3 objectToMouse, float theta, int speedLimit)
        {
            if (Mathf.Abs(theta - prevTheta) > speedLimit)
            {
                return speedLimit;
            }
            return (theta - prevTheta);
        }

        public float GetRotations() {

            return Mathf.Floor(accumulatedAngle / 360);
            

        }




}

