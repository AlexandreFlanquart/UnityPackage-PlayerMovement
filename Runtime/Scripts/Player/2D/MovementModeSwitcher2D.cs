using System;
using MyUnityPackage.Toolkit;
using UnityEngine;
using UnityEngine.AI;

namespace MyUnityPackage.Controller
{
    /// <summary>
    /// Switches the 2D player between its two movement modes at runtime:
    /// <see cref="MovementMode.ClickToMove"/> (NavMeshAgent driven by
    /// <see cref="ClickToMoveController2D"/>) and <see cref="MovementMode.Direct"/>
    /// (dynamic Rigidbody2D driven by <see cref="PlayerController2D"/>).
    /// The handover is made safe by stopping the outgoing controller, swapping the
    /// Rigidbody2D body type, and warping the agent back onto the NavMesh when entering
    /// ClickToMove mode — the switch is refused (mode kept) when no NavMesh lies within
    /// <see cref="maxSnapRadius"/>. Also swaps the animator source, and implements
    /// <see cref="IMovementControl"/> by delegating to the active controller so
    /// mode-agnostic systems (dialogue freeze, cutscenes) keep working across switches.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MovementModeSwitcher2D : MonoBehaviour, IMovementControl
    {
        public enum MovementMode { ClickToMove, Direct }

        [Header("Controllers")]
        [Tooltip("Click-to-move controller (NavMeshAgent). Resolved on this GameObject if left empty.")]
        [SerializeField] private ClickToMoveController2D clickController;
        [Tooltip("Direct input controller (Rigidbody2D). Resolved on this GameObject if left empty.")]
        [SerializeField] private PlayerController2D directController;

        [Header("Animator sources (optional)")]
        [Tooltip("Animator source used in ClickToMove mode. Resolved in children if left empty.")]
        [SerializeField] private IAAnimator2D agentAnimator;
        [Tooltip("Animator source used in Direct mode. Resolved in children if left empty.")]
        [SerializeField] private PlayerAnimator2D directAnimator;

        [Header("Switch")]
        [SerializeField] private MovementMode initialMode = MovementMode.ClickToMove;
        [Tooltip("Max distance used to snap the player back onto the NavMesh when entering ClickToMove mode. If no NavMesh lies within this radius, the switch is refused and the current mode is kept.")]
        [SerializeField, Min(0.1f)] private float maxSnapRadius = 1.5f;
        [Tooltip("Toggle the mode with the Jump input (unused by the 2D controllers). Cached when the component is enabled; disable/enable to apply runtime changes.")]
        [SerializeField] private bool toggleOnJump = true;

        /// <summary>Currently active movement mode.</summary>
        public MovementMode CurrentMode => _currentMode;

        /// <summary>Raised after a successful switch, with the new mode.</summary>
        public event Action<MovementMode> OnModeChanged;

        /// <summary>Raised when a requested switch is refused (e.g. no NavMesh under the player).</summary>
        public event Action<MovementMode> OnModeRefused;

        private NavMeshAgent _agent;
        private Rigidbody2D _body;
        private IPlayerMovement _input;
        private MovementMode _currentMode;
        private bool _initialized;   // true once a mode has been applied (Start)

        // Common control surface of the currently active controller.
        private IMovementControl ActiveControl =>
            _currentMode == MovementMode.ClickToMove ? clickController : (IMovementControl)directController;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _body = GetComponent<Rigidbody2D>();
            _input = GetComponent<IPlayerMovement>();

            if (clickController == null) clickController = GetComponent<ClickToMoveController2D>();
            if (directController == null) directController = GetComponent<PlayerController2D>();
            if (agentAnimator == null) agentAnimator = GetComponentInChildren<IAAnimator2D>(true);
            if (directAnimator == null) directAnimator = GetComponentInChildren<PlayerAnimator2D>(true);

            if (clickController == null || directController == null || _agent == null || _body == null)
                MUPLogger.Error("MovementModeSwitcher2D needs ClickToMoveController2D, PlayerController2D, a NavMeshAgent and a Rigidbody2D on this GameObject.", this);

            // Direct mode drives a top-down dynamic body: gravity and torque must stay out
            // whatever the prefab says, otherwise the player falls on the first switch.
            if (_body != null)
            {
                _body.gravityScale = 0f;
                _body.freezeRotation = true;
            }
        }

        private void OnEnable()
        {
            if (toggleOnJump && _input != null)
                _input.OnJumpPressed += ToggleMode;
        }

        private void OnDisable()
        {
            if (toggleOnJump && _input != null)
                _input.OnJumpPressed -= ToggleMode;
        }

        private void Start()
        {
            // Enforce a coherent initial state whatever the prefab checkboxes say.
            if (!ApplyMode(initialMode) && initialMode == MovementMode.ClickToMove)
            {
                MUPLogger.Warning("Initial ClickToMove mode unavailable (no NavMesh found), falling back to Direct mode.", this);
                ApplyMode(MovementMode.Direct);
            }
        }

        /// <summary>Switches to the other mode. See <see cref="SetMode"/> for the refusal rules.</summary>
        public void ToggleMode()
        {
            SetMode(_currentMode == MovementMode.ClickToMove ? MovementMode.Direct : MovementMode.ClickToMove);
        }

        /// <summary>
        /// Switches to <paramref name="mode"/>. Returns false when the switch is refused
        /// (entering ClickToMove with no NavMesh within <see cref="maxSnapRadius"/>);
        /// the current mode is then left untouched.
        /// </summary>
        public bool SetMode(MovementMode mode)
        {
            if (_initialized && mode == _currentMode)
                return true;

            return ApplyMode(mode);
        }

        private bool ApplyMode(MovementMode mode)
        {
            // A frozen player (dialogue, cutscene...) must stay frozen across the switch.
            bool keepMovementEnabled = !_initialized || ActiveControl == null || ActiveControl.MovementEnabled;

            bool applied = mode == MovementMode.ClickToMove ? EnterClickToMove() : EnterDirect();
            if (!applied)
            {
                OnModeRefused?.Invoke(mode);
                return false;
            }

            _currentMode = mode;
            _initialized = true;

            // Propagate unconditionally: the incoming controller may hold a stale frozen
            // state from before a previous switch (each controller keeps its own flag).
            ActiveControl?.SetMovementEnabled(keepMovementEnabled);

            OnModeChanged?.Invoke(mode);
            return true;
        }

        private bool EnterClickToMove()
        {
            if (_agent == null)
                return false;

            // The player may have left the NavMesh in direct mode: find the closest point
            // BEFORE touching anything, so a refusal leaves the current mode fully intact.
            if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, maxSnapRadius, _agent.areaMask))
            {
                MUPLogger.Warning($"ClickToMove switch refused: no NavMesh within {maxSnapRadius} units of {transform.position}.", this);
                return false;
            }

            if (directController != null)
            {
                directController.Stop();
                directController.enabled = false;
            }

            if (_body != null)
            {
                // Kinematic first: the agent owns the transform from here, physics must not
                // integrate leftover velocity nor depenetrate against the warp target.
                _body.linearVelocity = Vector2.zero;
                _body.bodyType = RigidbodyType2D.Kinematic;
            }

            // Warp is the authoritative agent placement: moves the transform and registers
            // the agent on the mesh in one step (SetDestination is valid right after).
            _agent.enabled = true;
            _agent.Warp(hit.position);
            Physics2D.SyncTransforms(); // colliders must follow the teleport within this frame

            if (clickController != null)
                clickController.enabled = true;

            SwapAnimators(directActive: false);
            MUPLogger.Info("Movement mode switched to ClickToMove.", this);
            return true;
        }

        private bool EnterDirect()
        {
            if (clickController != null)
            {
                clickController.Stop(); // clears the path + isStopped: no residual steering
                clickController.enabled = false;
            }

            // A live agent would fight the dynamic body for the transform every frame.
            if (_agent != null)
                _agent.enabled = false;

            if (_body != null)
            {
                _body.bodyType = RigidbodyType2D.Dynamic;
                _body.linearVelocity = Vector2.zero;
            }

            if (directController != null)
                directController.enabled = true;

            SwapAnimators(directActive: true);
            MUPLogger.Info("Movement mode switched to Direct.", this);
            return true;
        }

        // Only one DirectionalAnimator2D may drive the shared Animator at a time.
        // PlayerAnimator2D clears its sprint/crouch bools in its own OnDisable.
        private void SwapAnimators(bool directActive)
        {
            if (agentAnimator != null) agentAnimator.enabled = !directActive;
            if (directAnimator != null) directAnimator.enabled = directActive;
        }

        // ---- IMovementControl -----------------------------------------

        /// <inheritdoc />
        public bool MovementEnabled => ActiveControl != null && ActiveControl.MovementEnabled;

        /// <inheritdoc />
        public bool IsMoving => ActiveControl != null && ActiveControl.IsMoving;

        /// <inheritdoc />
        public void SetMovementEnabled(bool enabled) => ActiveControl?.SetMovementEnabled(enabled);

        /// <inheritdoc />
        public void Stop() => ActiveControl?.Stop();
    }
}
