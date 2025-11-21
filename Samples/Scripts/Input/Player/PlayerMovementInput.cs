using System;
using MyUnityPackage.Toolkit;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyUnityPackage.Controller
{
    public class PlayerMovementInput : MonoBehaviour, IPlayerMovement, PlayerControls.IPlayerMovementActions
    {
        /*
        public Vector2 MovementInput { get => movementInput;  set => movementInput = value; }
        public Vector2 LookInput { get => lookInput;  set => lookInput = value; }
        public bool IsCrounching { get => isCrounching;  set => isCrounching = value; }
        public bool JumpPressed { get => jumpPressed;  set => jumpPressed = value; }
        public bool IsSprinting { get => isSprinting;  set => isSprinting = value; }

        private Vector2 movementInput ;
        private Vector2 lookInput ;
        private bool isCrounching ;
        private bool jumpPressed ;
        private bool isSprinting ;
    
        void LateUpdate()
        {
            JumpPressed = false;
        }
*/
        public event Action OnJumpEvent;
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
            OnJumpEvent?.Invoke();
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
            if(context.performed)
                isPressed = true;   
            if(context.canceled)
                isPressed = false;   
            OnSprintEvent?.Invoke(isPressed);       
        }
    }
}

