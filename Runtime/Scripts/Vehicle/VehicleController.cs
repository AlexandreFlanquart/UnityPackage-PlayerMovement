using System;
using MyUnityPackage.Toolkit;
using UnityEngine;

namespace MyUnityPackage.Controller
{
    public class VehicleController : MonoBehaviour
    {
        Rigidbody carRb;
        
        [Header("Component")]
        [SerializeField] public WheelController[] wheelList;
        IVehicleInput vehicleInput;

        [Header("Motor")]
        [Min(0f)]
        public float CarTopSpeed = 200;
        [Min(0f)]
        public float MotorTorque = 2000f;

        [Min(0f)]
        public float CarTopSpeedReverse = 20f;
        [Min(0f)]
        public float BrakeTorque = 2000f;
        public AnimationCurve MotorCurve;// {get;set;}

        [Header("CenterOfMass")]
        float centreOfGravityOffset = -1;

        [Header("FixedUpdateVar")]
        public float availableTorque;
        public float currentMotorTorque;
        float forwardSpeed;
        float speedFactor;
        float vInput;
        float hInput;
        float steeringInput = 0;

        float speedFactorReverse = 0;
        float availableTorqueReverse = 0;
        float currentMotorTorqueReverse = 0;

        [Header("Headlight")]
        [SerializeField] HeadlightHandler headlightHandler;

        //[Header("VFX")]
        //driftEffect;
        void Start()
        {
            carRb = GetComponent<Rigidbody>();
            vehicleInput = GetComponent<IVehicleInput>();

            Vector3 centerOfMass = carRb.centerOfMass;
            centerOfMass.y += centreOfGravityOffset;
            carRb.centerOfMass = centerOfMass;

            // Get all wheel components attached to the car
            wheelList = GetComponentsInChildren<WheelController>();

        }
        void /*Fixed*/Update()
        {
            // Get player input for acceleration and steering
            vInput = vehicleInput.MovementInput.y; // Forward/backward input
            hInput = vehicleInput.MovementInput.x; // Steering input
            steeringInput = Mathf.Lerp(steeringInput,hInput,0.4f);
            // Calculate current speed along the car's forward axis
            forwardSpeed = Vector3.Dot(transform.forward, carRb.linearVelocity);
            speedFactor = Mathf.InverseLerp(0, CarTopSpeed, Mathf.Abs(forwardSpeed)); // Normalized speed factor
            availableTorque = MotorCurve.Evaluate(speedFactor);

            //Reverse
            speedFactorReverse = Mathf.InverseLerp(0, CarTopSpeedReverse, Mathf.Abs(forwardSpeed)); // Normalized speed factor
            availableTorqueReverse = MotorCurve.Evaluate(speedFactorReverse);
             // Reduce motor torque and steering at high speeds for better handling
            currentMotorTorque = Mathf.Lerp(MotorTorque, 0, availableTorque);
            currentMotorTorqueReverse = Mathf.Lerp(MotorTorque,0,availableTorqueReverse);

            // Determine if the player is accelerating or trying to reverse
            bool isAccelerating = Mathf.Sign(vInput) == Mathf.Sign(forwardSpeed);
            bool isBreaking = false;
            bool isReversing = false;
            
            foreach (var wheel in wheelList)
            {
                wheel.Drift(hInput,vehicleInput.IsDrifting);
                wheel.Steer(steeringInput,carRb.linearVelocity.magnitude);
                
                if(isAccelerating)
                {
                    if(vInput>=0)
                    {
                         wheel.Acceleration(vInput,currentMotorTorque);
                    }
                    else
                    {
                        wheel.Reverse(vInput,currentMotorTorqueReverse);
                        isReversing = true;
                    }
                        
                }
                else if (vInput != 0)
                {
                    wheel.Break(vInput,BrakeTorque);
                    isBreaking = true;
                }
                    
                wheel.UpdatePosition();
            }
            MUPLogger.Info(isAccelerating + " : " + isBreaking + " : " + isReversing);
            if(isBreaking)
            {
               headlightHandler.BreakLightOn();
            }
            else
            {
                headlightHandler.BreakLightOff();
            }
            if(isReversing)
                headlightHandler.ReverseLightOn();
            else
                headlightHandler.ReverseLightOff();
        }   

        #region RETURN_VEHICLE

        private bool CheckIsFlicked()
        {
            return transform.localEulerAngles.z>80 && transform.localEulerAngles.z<280; 
        }

        #endregion
       // public float GetMotorTorque() => motorTorque;
        //public float GetBrakeTorque() => brakeTorque;
        public float GetSpeed() => /*wheelList[0].wheelCollider.rpm *wheelList[0].wheelCollider.radius *2f * Mathf.PI / 10f*/ carRb.linearVelocity.magnitude * 3.6f;
        public bool isGoingForward() => Mathf.Sign(vInput) >= 0;
    }

    
    
}