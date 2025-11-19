using MyUnityPackage.Controller;
using MyUnityPackage.Toolkit;
using UnityEngine;

[RequireComponent(typeof(VehicleController))]
public class ApplyMotorSO : MonoBehaviour
{

    [SerializeField] MotorSO motorSO;


    void Awake()
    {
        VehicleController controller = GetComponent<VehicleController>();
        if (controller == null)
        {
            MUPLogger.Error("Controller not detected !");
            return;
        }
        if(motorSO == null)
        {
            MUPLogger.Error("MotorSO is not set !");
            return;
        }
        controller.CarTopSpeed = motorSO.carTopSpeed;
        controller.MotorTorque = motorSO.motorTorque;
        controller.BrakeTorque = motorSO.brakeTorque;
        controller.MotorCurve = motorSO.motorCurve;

    }
}
