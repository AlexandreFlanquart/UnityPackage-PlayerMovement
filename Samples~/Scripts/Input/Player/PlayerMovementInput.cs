using System;
using MyUnityPackage.Toolkit;
using UnityEngine;
using UnityEngine.InputSystem;
using MyUnityPackage.Controller;

namespace MyUnityPackage.ControllerSample
{
    public class PlayerMovementInput : MonoBehaviour, IPlayerMovement, PlayerControls.IPlayerMovementActions
    {
        // Jump events
        public event Action OnJumpPressed;
        public event Action OnJumpReleased;

        // Movement input (WASD / left stick)
        public event Action<Vector2> OnMoveEvent;

        // Look input (mouse delta / right stick)
        public event Action<Vector2> OnLookEvent;

        // Crouch events
        public event Action OnCrouchStarted;
        public event Action OnCrouchCanceled;

        // Sprint events
        public event Action OnSprintStarted;
        public event Action OnSprintCanceled;
        
        public event Action<Vector2> OnClickPressed;

        private PlayerControls _playerControls;

        private void OnEnable()
        {
            // Get PlayerControls instance from your input manager singleton
            _playerControls = PlayerInputManager.Instance?.PlayerControls;

            if (_playerControls == null)
            {
                MUPLogger.Error("PlayerControls is not set on PlayerInputManager. PlayerMovementInput will not receive inputs.");
                return;
            }

            // Enable PlayerMovement action map and register callbacks
            _playerControls.PlayerMovement.Enable();
            _playerControls.PlayerMovement.SetCallbacks(this);
        }

        private void OnDisable()
        {
            if (_playerControls == null)
                return;

            // Disable PlayerMovement action map and unregister callbacks
            _playerControls.PlayerMovement.Disable();
            _playerControls.PlayerMovement.RemoveCallbacks(this);
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            // Continuous movement vector (WASD / left stick)
            Vector2 moveInput = context.ReadValue<Vector2>();
            OnMoveEvent?.Invoke(moveInput);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            // Continuous look input (mouse delta / right stick)
            Vector2 lookInput = context.ReadValue<Vector2>();
            OnLookEvent?.Invoke(lookInput);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            // We separate "pressed" and "released":
            // - performed => button pressed (useful for jump, double jump, jump buffering, etc.)
            // - canceled  => button released (useful for variable jump height, etc.)
            if (context.performed)
            {
                MUPLogger.Info("Jump input performed.");
                OnJumpPressed?.Invoke();
            }
            else if (context.canceled)
            {
                MUPLogger.Info("Jump input canceled.");
                OnJumpReleased?.Invoke();
            }
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            // Crouch is a button: we expose start and cancel events.
            // Subscribers can decide if it's a hold or a toggle.
            if (context.started)
            {
                MUPLogger.Info("Crouch started.");
                OnCrouchStarted?.Invoke();
            }
            else if (context.canceled)
            {
                MUPLogger.Info("Crouch canceled.");
                OnCrouchCanceled?.Invoke();
            }
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            // Sprint is also a button: we expose start and cancel,
            // and the consumer chooses hold/toggle behavior.
            if (context.started || context.performed)
            {
                MUPLogger.Info("Sprint started/performed.");
                OnSprintStarted?.Invoke();
            }
            else if (context.canceled)
            {
                MUPLogger.Info("Sprint canceled.");
                OnSprintCanceled?.Invoke();
            }
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            Vector2 screenPos = Input.mousePosition;
            MUPLogger.Info($"Click input performed at screen pos {screenPos}.");

            OnClickPressed?.Invoke(screenPos);
        }
    }
}
