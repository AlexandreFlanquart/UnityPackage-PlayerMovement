using UnityEngine;
using UnityEngine.InputSystem;


namespace MyUnityPackage.Controller
{
    [DefaultExecutionOrder(-3)]
    public class PlayerMovementInput : MonoBehaviour, PlayerControls.IPlayerMovementActions
    {
        public Vector2 MovementInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool IsCrounching { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool IsSprinting { get; private set; }

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
            if(!context.performed)
                return;

            LookInput = context.ReadValue<Vector2>();
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            if(!context.performed)
                return;

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

