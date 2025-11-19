using MyUnityPackage.Toolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace MyUnityPackage.Controller
{
    public class WheelController : MonoBehaviour
    {
        [SerializeField] public WheelCollider wheelCollider;
        [SerializeField] public Transform meshTransform;
        Vector3 position;
        Quaternion rotation;
        [Header("Controls")]
        public bool steerable;
        public bool motorized;

        public bool isGrounded;
        WheelHit hit;
 
        [Header("Steering")]
        [SerializeField] public float steeringRange;
        [SerializeField] public float steeringRangeAtMaxSpeed;
        [SerializeField] AnimationCurve steeringCurve;

        [Header("Drift")]
        [SerializeField] float driftFactor = 0.75f;//0 full drift ,1 no drift

        public void UpdatePosition()
        {
            wheelCollider.GetWorldPose(out position,out rotation);
            meshTransform.position = position;
            meshTransform.rotation = rotation;
        }
        public void IsGrounded()
        {
             if(wheelCollider.GetGroundHit(out hit))
                isGrounded = true;
             else 
                isGrounded = false;
        }
        #region CONTROL
        public void Drift(float hInput,bool isDrifting)
        {  
            if(!steerable)
            {
                WheelFrictionCurve fr = wheelCollider.sidewaysFriction;
                //float slip = wheelCollider.rpm * 0.01f; // approximate slip for effect
                fr.stiffness = isDrifting?Mathf.Lerp(0.2f, 1f, Mathf.Abs(hInput) * driftFactor):1;
                wheelCollider.sidewaysFriction = fr;
            }
        }
   
        public void Steer(float steerInput,float speedFactor)
        {   
            if (steerable)
            {
                float steeringAngle = steerInput* steeringCurve.Evaluate(speedFactor);
                wheelCollider.steerAngle =steeringAngle;
            }
        }
        public void Acceleration(float accelInput,float currentMotorTorque)
        {
            // Apply torque to motorized wheels
            if (motorized)
            {
                wheelCollider.motorTorque = accelInput * currentMotorTorque;
            }
            // Release brakes when accelerating
            wheelCollider.brakeTorque = 0f;
        }
        public void Reverse(float accelInput,float currentMotorReverseTorque)
        {
             // Apply torque to motorized wheels
            if (motorized)
            {
                wheelCollider.motorTorque = accelInput * currentMotorReverseTorque;
                //MUPLogger.Info("MotorTorque " + wheelCollider.motorTorque);
            }
            // Release brakes when accelerating
            wheelCollider.brakeTorque = 0f;
        }
        public void Break(float accelInput,float brakeTorque)
        {
            // Apply brakes when reversing direction
            wheelCollider.motorTorque = 0f;
            wheelCollider.brakeTorque = Mathf.Abs(accelInput) * brakeTorque;
        }
        #endregion
  
    }
}