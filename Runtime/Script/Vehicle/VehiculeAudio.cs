using MyUnityPackage.Controller;
using MyUnityPackage.Toolkit;
using UnityEngine;

public class VehiculeAudio : MonoBehaviour
{
    [Header("Sounds")]

    [SerializeField] AudioSource runningSource;
    [SerializeField] AudioSource reverseSource;
    [SerializeField] AudioSource idleSource;
    [SerializeField] AudioSource klaxonSource;
    [SerializeField] AudioSource driftSource;
    
    float runningMaxVol = 0.7f;
    float runningMaxPitch = 1;

    float deltaSpeed;

    VehicleController vehicleController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vehicleController = transform.parent.GetComponent<VehicleController>();
        IVehicleInput vehicleInput= transform.parent.GetComponent<IVehicleInput>();
        vehicleInput.OnKlaxonAction += OnKlaxon;
        vehicleInput.OnDriftAction += OnDrift;
    }

    // Update is called once per frame
    void Update()
    {
       if(vehicleController == null)
            return; 
        bool isGoingForward = vehicleController.isGoingForward();
        deltaSpeed = vehicleController.GetSpeed();
        if(isGoingForward)
        {
            reverseSource.volume = 0;
            runningSource.volume = Mathf.Lerp(.3f,runningMaxVol,deltaSpeed);
            runningSource.pitch = Mathf.Lerp(.3f,runningMaxPitch,deltaSpeed);
        }
        else
        {
            runningSource.volume = 0;
            reverseSource.volume = Mathf.Lerp(.3f,runningMaxVol,deltaSpeed);
            reverseSource.pitch = Mathf.Lerp(.3f,runningMaxPitch,deltaSpeed);
        }
    }

    public void OnKlaxon(bool isActive)
    {
        if(isActive)
            klaxonSource.Play();
        else
            klaxonSource.Stop();
    }
    public void OnDrift(bool isDrifting)
    {
        if(isDrifting)
            driftSource.Play();
        else   
            driftSource.Stop();
    }
}
