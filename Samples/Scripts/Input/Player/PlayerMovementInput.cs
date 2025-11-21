using System;
using MyUnityPackage.Toolkit;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.MeshOperations;

namespace MyUnityPackage.Controller
{
    public class PlayerMovementInput : MonoBehaviour, IPlayerMovement, PlayerControls.IPlayerMovementActions
    {
        public event Action<bool> OnJumpEvent;
        public event Action<Vector2> OnMoveEvent;
        public event Action OnCrouchEvent;
        public event Action<Vector2> OnLookEvent;
        public event Action<bool> OnSprintEvent;

        void OnEnable()
        {
            if(PlayerInputManager.Instance.PlayerControls == null)
            {
                MUPLogger.Error("Players controls is not set !");
                return;
            }
            PlayerInputManager.Instance.PlayerControls.PlayerMovement.Enable();
            PlayerInputManager.Instance.PlayerControls.PlayerMovement.SetCallbacks(this);
        }

        void OnDisable()
        {
            if(PlayerInputManager.Instance.PlayerControls == null)
            {
                MUPLogger.Error("Players controls is not set !");
                return;
            }
            PlayerInputManager.Instance.PlayerControls.PlayerMovement.Disable();
            PlayerInputManager.Instance.PlayerControls.PlayerMovement.RemoveCallbacks(this);
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            MUPLogger.Info("Crouch input detected : " + context.performed);
            OnCrouchEvent?.Invoke();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            MUPLogger.Info("Jump input detected : " + context.performed);
            bool isPressed = false;
            if(context.started)
                isPressed = true;
            else if(context.performed)
                return;   
            else if(context.canceled)
                isPressed = false;   
            OnJumpEvent?.Invoke(isPressed);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            //MUPLogger.Info("Look input detected : " + context.ReadValue<Vector2>());
            OnLookEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            MUPLogger.Info("Move input detected : " + context.ReadValue<Vector2>());
            OnMoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            MUPLogger.Info("Sprint input detected : " + context.performed);
            bool isPressed = false;
            if(context.started)
                isPressed = true;
            else if(context.performed)
                return;   
            else if(context.canceled)
                isPressed = false;   
            OnSprintEvent?.Invoke(isPressed);       
        }
    }
}

