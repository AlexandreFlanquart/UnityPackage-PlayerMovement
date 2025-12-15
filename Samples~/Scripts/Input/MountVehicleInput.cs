/*
using MyUnityPackage.Controller;
using MyUnityPackage.Toolkit;
using UnityEngine;
using UnityEngine.InputSystem;
using MyUnityPackage.Controller;


[DefaultExecutionOrder(-2)]
public class MountVehicleInput : MonoBehaviour, PlayerControls.IVehicleRideActions
{
    [Header("Variable controls")]
    public float height;
    public float radius;
    public LayerMask layerMask;
     void OnEnable()
    {
        if(PlayerInputManager.Instance.PlayerControls == null)
        {
            Debug.Log("Players controls is not set !");
            return;
        }
        PlayerInputManager.Instance.PlayerControls.VehicleRide.Enable();
        PlayerInputManager.Instance.PlayerControls.VehicleRide.SetCallbacks(this);
    }
    void OnDisable()
    {
        if(PlayerInputManager.Instance.PlayerControls == null)
        {
            Debug.Log("Players controls is not set !");
            return;
        }
        PlayerInputManager.Instance.PlayerControls.VehicleRide.Disable();
        PlayerInputManager.Instance.PlayerControls.VehicleRide.RemoveCallbacks(this);
    }
    public void OnInside(InputAction.CallbackContext context)
    {
        Debug.Log("PERFORMED MOUNT VEHICLE");
        if(context.performed)
        {
            RaycastHit[] carNext = Physics.CapsuleCastAll(transform.position+new Vector3(0,height/2f,0), transform.position-new Vector3(0,height/2f,0),radius,Vector3.up,5f,layerMask);
            //RaycastHit[] carNext = Physics.SphereCastAll(transform.position,10f,Vector3.up,10f,layerMask);
            MUPLogger.Info("Car lenght " + carNext.Length);
            GameObject getNearestCar = null;

            if(carNext.Length <= 0)
                return;

            foreach(RaycastHit car in carNext)
            {
                
                if(getNearestCar == null || Vector3.Distance(transform.position, car.transform.position) < Vector3.Distance(transform.position,getNearestCar.transform.position))
                {
                    getNearestCar = car.transform.gameObject;
                    continue;
                }
            }
            if(getNearestCar != null)
            {
                MUPLogger.Info("Interact Vehicle");
                getNearestCar.GetComponentInChildren<VehiculeHandler>().InteractVehicule(transform);
                ServiceLocator.GetService<CarUI>().vehicleController = getNearestCar.GetComponent<VehicleController>();
            }
            else
                MUPLogger.Info("Pas de véhicle proche");
            MUPLogger.Info(getNearestCar.transform.name + ":" + getNearestCar.transform.gameObject.layer.ToString());
        }
    }
  

}
*/