using MyUnityPackage.Controller;
using UnityEngine;

public class VehicleEffects : MonoBehaviour
{
    [SerializeField] WheelController rearLeft;
    [SerializeField] WheelController rearRight;

    void Start()
    {
        IVehicleInput vehicleInput= GetComponent<IVehicleInput>();
        vehicleInput.OnDriftAction += OnDrift;
    }
    void OnDrift(bool isActive)
    {
        rearLeft.GetComponentInChildren<TrailRenderer>().emitting = isActive;
        rearRight.GetComponentInChildren<TrailRenderer>().emitting = isActive;
    }
}
