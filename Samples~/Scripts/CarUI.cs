using MyUnityPackage.Controller;
using TMPro;
using UnityEngine;

public class CarUI : MonoBehaviour
{
    [SerializeField] public VehicleController vehicleController;
    [SerializeField] private TextMeshProUGUI spdText;
    [SerializeField] private TextMeshProUGUI motorTorqueText;
    [SerializeField] private TextMeshProUGUI availableTorqueText;
    [SerializeField] private TextMeshProUGUI breakTorqueText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(vehicleController != null)
        {
            spdText.text = "Speed : " + (int)vehicleController.GetSpeed();
            motorTorqueText.text = "MotorTorque : " + (int)vehicleController.MotorTorque;
            availableTorqueText.text = "AvailableTorque : " + /*(int)*/vehicleController.currentMotorTorque;
            breakTorqueText.text = "BreakTorque : " + (int)vehicleController.BrakeTorque;
        }
           
    }
}
