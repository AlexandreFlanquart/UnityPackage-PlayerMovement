using System;
using UnityEngine;
using UnityEngine.InputSystem;

/*
namespace MyUnityPackage.Controller
{
    public enum ActionMap
    {
        PLAYER,
        UI
    }
    [DefaultExecutionOrder(-3)]
    public class InputManager : MonoBehaviour
    {
        public event Action<Vector2> OnPressDirection;
        public event Action<Vector2> OnLookDirection;

        public event Action OnJumpPressed;
        public event Action OnPressInteract;
        public event Action OnCrouchPressed;
        public event Action OnCrouchCanceled;

        public PlayerControls playerControls;
        private PlayerInput playerInput;
        public Vector2 movementInput = Vector2.zero;
        public Vector2 lookInput = Vector2.zero;
        public bool canSprint;
        public bool jumpPressed;
        static InputManager instance;

        #region UNITY_FUNCTION
        private void Awake()
        {
            if (instance == null)
                instance = this;

            Debug.Log("InputManager Start");
            playerInput = GetComponent<PlayerInput>();
            playerControls = new PlayerControls();
        }


        private void OnEnable()
        {
            playerControls.Player.Enable();

            playerControls.Player.Movement.performed += MovementPerformed;
            playerControls.Player.Movement.canceled += MovementCanceled;

            playerControls.Player.Look.performed += LookPerformed;
            playerControls.Player.Look.canceled += LookCanceled;

            playerControls.Player.Interact.performed += InteractPerformed;
        
            playerControls.Player.Jump.performed += JumpPerformed;

            playerControls.Player.Sprint.performed += SprintPerformed;

            playerControls.Player.Crouch.performed += CrouchPerformed;
            playerControls.Player.Crouch.canceled += CrouchCanceled;
        }

            

        private void OnDisable()
        {
            playerControls.Player.Disable();

            playerControls.Player.Movement.performed -= MovementPerformed;
            playerControls.Player.Movement.canceled -= MovementCanceled;

            playerControls.Player.Look.performed -= LookPerformed;
            playerControls.Player.Look.canceled -= LookCanceled;

            playerControls.Player.Interact.performed -= InteractPerformed;
        
            playerControls.Player.Jump.performed -= JumpPerformed;

            playerControls.Player.Sprint.performed -= SprintPerformed;

            playerControls.Player.Crouch.performed -= CrouchPerformed;
            playerControls.Player.Crouch.canceled -= CrouchCanceled;
        }

        void LateUpdate()
        {
            jumpPressed = false;
        }
        #endregion
        public static InputManager GetInstance()
        {
            return instance;
        }

        #region CALLBACK
        private void JumpPerformed(InputAction.CallbackContext context)
        {
            Debug.Log("Jump performed");
            if(!context.performed)
                return;

            jumpPressed = true;
            OnJumpPressed?.Invoke();
        }

        private void CrouchPerformed(InputAction.CallbackContext context)
        {
            OnCrouchPressed?.Invoke();
        }
        private void CrouchCanceled(InputAction.CallbackContext context)
        {
            OnCrouchCanceled?.Invoke();
        }
        private void MovementPerformed(InputAction.CallbackContext context)
        {
            // Debug.Log("Movement Performed : " + context.ReadValue<Vector2>());
            movementInput = context.ReadValue<Vector2>();
        }

        private void MovementCanceled(InputAction.CallbackContext context)
        {
            //Debug.Log("Movement Canceled");
            movementInput = Vector2.zero;
        }

        private void LookPerformed(InputAction.CallbackContext context)
        {
            // Debug.Log("look Performed : " + context.ReadValue<Vector2>());
            lookInput = context.ReadValue<Vector2>();
        }

        private void LookCanceled(InputAction.CallbackContext context)
        {
            //Debug.Log("look Canceled");
            lookInput = Vector2.zero;
        }

        private void InteractPerformed(InputAction.CallbackContext context)
        {
            Debug.Log("Interact Performed");
            OnPressInteract?.Invoke();
        }
        private void SprintPerformed(InputAction.CallbackContext context)
        {
            if(context.performed)
                canSprint = true;
            else
                canSprint = false;
        }

        #endregion
 
    }
}
*/

