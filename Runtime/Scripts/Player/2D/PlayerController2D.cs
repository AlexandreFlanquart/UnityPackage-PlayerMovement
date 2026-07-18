using MyUnityPackage.Toolkit;
using UnityEngine;

namespace MyUnityPackage.Controller
{
    public enum InputToggleMode
    {
        Hold,
        Toggle
    }

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(IPlayerMovement))]
    public class PlayerController2D : MonoBehaviour, IMovementControl
    {
        // Squared velocity below which the player is considered idle (~0.01 units/s).
        private const float MovingEpsilonSqr = 0.0001f;

        private IPlayerMovement input;
        private Rigidbody2D rb;
        private bool movementEnabled = true;

        [Header("Movement Settings")]
        [SerializeField] private float baseSpeed = 5f;

        [Header("Sprint Settings")]
        [SerializeField] private InputToggleMode sprintMode = InputToggleMode.Hold;
        [SerializeField] private float sprintMultiplier = 1.5f;
        private bool sprintHeld;
        private bool sprintToggled;
        private bool IsSprinting => sprintMode == InputToggleMode.Hold ? sprintHeld : sprintToggled;

        [Header("Crouch Settings")]
        [SerializeField] private InputToggleMode crouchMode = InputToggleMode.Hold;
        [SerializeField] private float crouchMultiplier = 0.5f;
        private bool crouchHeld;
        private bool crouchToggled;
        private bool IsCrouching => crouchMode == InputToggleMode.Hold ? crouchHeld : crouchToggled;
       
        public bool IsSprintingPublic => IsSprinting;
        public bool IsCrouchingPublic => IsCrouching;       
        private Vector2 moveInput;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            input = GetComponent<IPlayerMovement>();
        }

        private void OnEnable()
        {
            if (input == null)
            {
                MUPLogger.Error("Input source does not implement IPlayerMovement.", this);
                return;
            }

            // Subscribe to input events
            input.OnMoveEvent += HandleMoveInput;
            input.OnSprintStarted += HandleSprintStart;
            input.OnSprintCanceled += HandleSprintCancel;
            input.OnCrouchStarted += HandleCrouchStart;
            input.OnCrouchCanceled += HandleCrouchCancel;
        }

        private void OnDisable()
        {
            if (input == null) return;

            // Unsubscribe from input events
            input.OnMoveEvent -= HandleMoveInput;
            input.OnSprintStarted -= HandleSprintStart;
            input.OnSprintCanceled -= HandleSprintCancel;
            input.OnCrouchStarted -= HandleCrouchStart;
            input.OnCrouchCanceled -= HandleCrouchCancel;
        }

        private void FixedUpdate()
        {
            // Frozen: keep the player still and ignore movement input.
            if (!movementEnabled)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            float speed = baseSpeed;

            if (IsSprinting)
                speed *= sprintMultiplier;

            if (IsCrouching)
                speed *= crouchMultiplier;

            Vector2 velocity = moveInput.normalized * speed;
            rb.linearVelocity = velocity;
        }

        // ---- IMovementControl -----------------------------------------

        /// <inheritdoc />
        public bool MovementEnabled => movementEnabled;

        /// <inheritdoc />
        public bool IsMoving => rb != null && rb.linearVelocity.sqrMagnitude > MovingEpsilonSqr;

        /// <inheritdoc />
        public void SetMovementEnabled(bool enabled)
        {
            if (movementEnabled == enabled)
                return;

            movementEnabled = enabled;

            if (!enabled)
                Stop();
        }

        /// <inheritdoc />
        /// <remarks>
        /// The buffered <c>moveInput</c> is intentionally kept: the Input System only sends
        /// events on value changes, so clearing it would leave a held key ignored after
        /// <see cref="SetMovementEnabled"/> is set back to <c>true</c>. The <c>movementEnabled</c>
        /// gate in <see cref="FixedUpdate"/> already zeroes the velocity every tick while disabled.
        /// </remarks>
        public void Stop()
        {
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
        }

        // Input Handlers ------------------------------------------

        private void HandleMoveInput(Vector2 value)
        {
            moveInput = value;
        }

        private void HandleSprintStart()
        {
            if (sprintMode == InputToggleMode.Hold)
                sprintHeld = true;
            else
                sprintToggled = !sprintToggled;
        }

        private void HandleSprintCancel()
        {
            if (sprintMode == InputToggleMode.Hold)
                sprintHeld = false;
        }

        private void HandleCrouchStart()
        {
            if (crouchMode == InputToggleMode.Hold)
                crouchHeld = true;
            else
                crouchToggled = !crouchToggled;
        }

        private void HandleCrouchCancel()
        {
            if (crouchMode == InputToggleMode.Hold)
                crouchHeld = false;
        }
    }
}
