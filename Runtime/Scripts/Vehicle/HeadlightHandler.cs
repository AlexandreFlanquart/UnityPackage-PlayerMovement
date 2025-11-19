using MyUnityPackage.Controller;
using MyUnityPackage.Toolkit;
using UnityEngine;

public class HeadlightHandler : MonoBehaviour
{

    [SerializeField] IVehicleInput vehicleInput;
    [Header("FrontLight")]
    [SerializeField] GameObject frontLeft;
    [SerializeField] GameObject frontRight;
    bool frontLightEnabled = false;
    [Header("BrakeLight")]
    [SerializeField] GameObject brakeLightOn;
    [SerializeField] GameObject brakeLightOff;
    bool breakLightOn = false;

    [Header("ReverseLight")]
    bool reverseLightIsOn = false;
    [SerializeField] GameObject reverseLightOn;
    [SerializeField] GameObject reverseLightOff;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vehicleInput = GetComponentInParent<IVehicleInput>();
        vehicleInput.OnHeadLightAction += OnHeadLight;
    }

    void OnHeadLight()
    {
        MUPLogger.Info("Light");
        frontLightEnabled = !frontLightEnabled;
        frontLeft.GetComponentInChildren<Light>().enabled = frontLightEnabled;
        frontRight.GetComponentInChildren<Light>().enabled = frontLightEnabled;
    }
    public void BreakLightOn()
    {
        if(breakLightOn)
            return;
        breakLightOn = true;
      brakeLightOn.SetActive(true);
      brakeLightOff.SetActive(false);
    }
    public void BreakLightOff()
    {
        if(!breakLightOn)
            return;
        breakLightOn = false;
        brakeLightOn.SetActive(false);
        brakeLightOff.SetActive(true);
    }
    public void ReverseLightOn()
    {
        if(reverseLightIsOn)
            return;
        reverseLightIsOn = true;
        reverseLightOn.SetActive(true);
        reverseLightOff.SetActive(false);
    }
    public void ReverseLightOff()
    {
        if(!reverseLightIsOn)
            return;
        reverseLightIsOn = false;
        reverseLightOn.SetActive(false);
        reverseLightOff.SetActive(true);
    }
}
