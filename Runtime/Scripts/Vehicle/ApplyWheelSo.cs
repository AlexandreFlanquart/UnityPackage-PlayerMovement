using MyUnityPackage.Controller;
using MyUnityPackage.Toolkit;
using UnityEngine;


[RequireComponent(typeof(VehicleController))]
public class ApplyWheelSo : MonoBehaviour
{
    [SerializeField] WheelSO wheelSO;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VehicleController controller = GetComponent<VehicleController>();
        if (controller == null)
        {
            MUPLogger.Error("Controller not detected !");
            return;
        }
        if(wheelSO == null)
        {
            MUPLogger.Error("WheelSO is not set !");
            return;
        }

        foreach(WheelController wheel in controller.wheelList)
        {
            wheel.wheelCollider.mass = wheelSO.mass;
            //Forward Friction
            WheelFrictionCurve forwardFriction = wheel.wheelCollider.forwardFriction;
            forwardFriction.extremumSlip = wheelSO.fExtremumSlip;
            forwardFriction.extremumValue = wheelSO.fExtremumValue;
            forwardFriction.asymptoteValue = wheelSO.fAsymptoteValue;
            forwardFriction.asymptoteSlip = wheelSO.fAsymptoteSlip;
            forwardFriction.stiffness = wheelSO.fStiffness;
            wheel.wheelCollider.forwardFriction = forwardFriction;

            //Sideways friction
            WheelFrictionCurve sidewaysFriction = wheel.wheelCollider.sidewaysFriction;
            sidewaysFriction.extremumSlip = wheelSO.sExtremumSlip;
            sidewaysFriction.extremumValue = wheelSO.sExtremumValue;
            sidewaysFriction.asymptoteValue = wheelSO.sAsymptoteValue;
            sidewaysFriction.asymptoteSlip = wheelSO.sAsymptoteSlip;
            sidewaysFriction.stiffness = wheelSO.sStiffness;
            wheel.wheelCollider.sidewaysFriction = sidewaysFriction;

            //Steering 
            wheel.steeringRange = wheelSO.steeringRange;
            wheel.steeringRangeAtMaxSpeed= wheelSO.steeringRangeAtMaxSpeed;
        }
    }

}
