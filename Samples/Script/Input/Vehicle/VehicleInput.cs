using UnityEngine;
using UnityEngine.InputSystem;
using MyUnityPackage.Controller;

using PlayerInputManager = MyUnityPackage.Controller.PlayerInputManager;
using System;
[DefaultExecutionOrder(-2)]
public class VehicleInput : MonoBehaviour,PlayerControls.IVehicleActions,IVehicleInput
{
    public Vector2 MovementInput { get => movementInput;  set => movementInput = value; }
    private Vector2 movementInput ;
    public static VehicleInput Instance{ get; private set;}
    public bool IsDrifting { get => isDrifting; set => isDrifting =  value; }
    public Action<bool> OnKlaxonAction { get; set; }
    public Action<bool> OnDriftAction { get ; set; }
    public Action OnHeadLightAction { get ; set; }

    private bool isDrifting ;

    void Start()
    {
        Instance = this;
    }
    public void EnableVehicleInput()
    {
        if(PlayerInputManager.Instance.PlayerControls == null)
        {
            Debug.Log("Players controls is not set !");
            return;
        }
        PlayerInputManager.Instance.PlayerControls.Vehicle.Enable();
        PlayerInputManager.Instance.PlayerControls.Vehicle.SetCallbacks(this);
    }
    public void DisableVehicleInput()
    {
        if(PlayerInputManager.Instance.PlayerControls == null)
        {
            Debug.Log("Players controls is not set !");
            return;
        }
        PlayerInputManager.Instance.PlayerControls.Vehicle.Disable();
        PlayerInputManager.Instance.PlayerControls.Vehicle.RemoveCallbacks(this);
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }
    public void OnDrifting(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            IsDrifting = true;
             OnDriftAction?.Invoke(IsDrifting);
        }
        else if(context.canceled)
        {
            IsDrifting = false;
            OnDriftAction?.Invoke(IsDrifting);
        }
            
       
    }
    public void OnKlaxon(InputAction.CallbackContext context)
    {
        if(context.performed)
            OnKlaxonAction?.Invoke(true);
        else if (context.canceled  )
            OnKlaxonAction?.Invoke(false);
    }

    public void OnHeadLight(InputAction.CallbackContext context)
    {
        if(context.performed)
            OnHeadLightAction?.Invoke();

    }
}


