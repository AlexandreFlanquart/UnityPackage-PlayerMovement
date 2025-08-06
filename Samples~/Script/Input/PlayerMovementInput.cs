using UnityEngine;
using UnityEngine.InputSystem;


namespace MyUnityPackage.Controller
{
    [DefaultExecutionOrder(-2)]
    public class PlayerMovementInput : MonoBehaviour,IPlayerMovement, PlayerControls.IPlayerMovementActions
    {
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

        void OnEnable()
        {
            if(PlayerInputManager.Instance.PlayerControls == null)
            {
                Debug.Log("Players controls is not set !");
                return;
            }
            PlayerInputManager.Instance.PlayerControls.PlayerMovement.Enable();
            PlayerInputManager.Instance.PlayerControls.PlayerMovement.SetCallbacks(this);
        }
        void OnDisable()
        {
            if(PlayerInputManager.Instance.PlayerControls == null)
            {
                Debug.Log("Players controls is not set !");
                return;
            }
            PlayerInputManager.Instance.PlayerControls.PlayerMovement.Disable();
            PlayerInputManager.Instance.PlayerControls.PlayerMovement.RemoveCallbacks(this);
        }
        public void OnCrouch(InputAction.CallbackContext context)
        {
            if(context.performed)
                IsCrounching = true;
            else if(context.canceled)
                IsCrounching = false;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if(!context.performed)
                return;

            JumpPressed = true;
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            MovementInput = context.ReadValue<Vector2>();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if(context.performed)
                IsSprinting = true;
            else if(context.canceled)
                IsSprinting = false;
            
        }
    }
}

