using System;
using MyUnityPackage.Toolkit;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace MyUnityPackage.Controller
{
    /// <summary>
    /// 2D click-to-move controller driving a <see cref="NavMeshAgent"/> from click input.
    /// Implements <see cref="IPlayerController2D"/> so consumers can pilot the player at
    /// runtime (enable/disable input, stop, pause/resume, move or teleport by code) without
    /// touching the agent directly. Disabling the component via <see cref="Behaviour.enabled"/>
    /// still unsubscribes from input as before (backward compatible).
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    public class ClickToMoveController2D : MonoBehaviour, IPlayerController2D
    {
        [Tooltip("2D ground layers used for the click raycast")]
        [SerializeField] private LayerMask groundMask = ~0;
        [Tooltip("Max distance allowed to \"snap\" the click point onto the NavMesh")]
        [SerializeField] private float sampleRadius = 1.0f;
        [Tooltip("Max ray length used to raycast the click against 2D ground colliders")]
        [SerializeField] private float rayDistance = 100f;
        [Tooltip("Optional: raised (in addition to the C# event) when a path completes naturally")]
        [SerializeField] private UnityEvent onDestinationReachedEvent;

        // Squared velocity below which the agent is considered idle (~0.01 units/s).
        private const float MovingEpsilonSqr = 0.0001f;

        private IPlayerMovement input;
        private NavMeshAgent agent;
        private Camera mainCamera;

        private bool movementEnabled = true;
        private bool hasActiveDestination;

        /// <inheritdoc />
        public bool MovementEnabled => movementEnabled;

        /// <inheritdoc />
        public bool IsMoving => agent != null && agent.hasPath && agent.velocity.sqrMagnitude > MovingEpsilonSqr;

        /// <inheritdoc />
        public event Action OnDestinationReached;

        private void Awake()
        {
            input = GetComponent<IPlayerMovement>();
            agent = GetComponent<NavMeshAgent>();
            mainCamera = Camera.main;

            // 2D-friendly agent setup
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.angularSpeed = 0f;
        }

        private void OnEnable()
        {
            if (input == null)
            {
                MUPLogger.Error("Input source does not implement IPlayerMovement.", this);
                return;
            }

            // Subscribe to click input events
            input.OnClickPressed += handleClickInput;
        }

        void OnDisable()
        {
            if (input == null) return;

            input.OnClickPressed -= handleClickInput;
        }

        private void Update()
        {
            // Fire OnDestinationReached once when a commanded/clicked path completes naturally.
            if (!hasActiveDestination || agent == null || !agent.isOnNavMesh)
                return;

            if (!agent.pathPending
                && agent.remainingDistance <= agent.stoppingDistance
                && agent.velocity.sqrMagnitude <= MovingEpsilonSqr)
            {
                hasActiveDestination = false;
                OnDestinationReached?.Invoke();
                onDestinationReachedEvent?.Invoke();
            }
        }

        private void handleClickInput(Vector2 screenPos)
        {
            // Ignore click input while movement is disabled (e.g. during a dialogue).
            if (!movementEnabled || !EnsureCamera())
                return;

            if (agent == null || !agent.isOnNavMesh)
                return;

            // Convert the screen position into a world-space ray
            Ray ray = mainCamera.ScreenPointToRay(screenPos);
            RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray, rayDistance, groundMask);

            // No ground collider under the click: ignore it. RaycastHit2D.point defaults to (0,0)
            // on a miss, which previously sent the agent walking toward the world origin.
            if (hit2D.collider == null)
                return;

            // Snap the hit point onto the NavMesh and move the agent
            if (NavMesh.SamplePosition(hit2D.point, out NavMeshHit navHit, sampleRadius, agent.areaMask))
            {
                agent.isStopped = false;
                agent.SetDestination(navHit.position);
                hasActiveDestination = true;
            }
        }

        /// <summary>
        /// Overrides the camera used to convert clicks to world rays. Useful when the main
        /// camera is created/replaced at runtime (additive scenes, camera swaps).
        /// </summary>
        /// <param name="camera">Camera used for click raycasts. If null, falls back to <see cref="Camera.main"/>.</param>
        public void SetCamera(Camera camera)
        {
            mainCamera = camera;
        }

        // Lazily (re)resolve the main camera so clicks keep working if it was recreated.
        private bool EnsureCamera()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            return mainCamera != null;
        }

        // ---- IPlayerController2D control API ----------------------------

        /// <inheritdoc />
        public void SetMovementEnabled(bool enabled)
        {
            if (movementEnabled == enabled)
                return;

            movementEnabled = enabled;

            // Disabling movement must freeze the player immediately (no glide to last click).
            if (!enabled)
                Stop();
        }

        /// <inheritdoc />
        [ContextMenu("Stop")]
        public void Stop()
        {
            hasActiveDestination = false;

            if (agent == null || !agent.isOnNavMesh)
                return;

            agent.isStopped = true;
            agent.ResetPath();
        }

        /// <inheritdoc />
        public void MoveTo(Vector3 worldPosition)
        {
            if (agent == null || !agent.isOnNavMesh)
                return;

            if (NavMesh.SamplePosition(worldPosition, out NavMeshHit navHit, sampleRadius, agent.areaMask))
            {
                agent.isStopped = false;
                agent.SetDestination(navHit.position);
                hasActiveDestination = true;
            }
        }

        /// <inheritdoc />
        public void Warp(Vector3 worldPosition)
        {
            if (agent == null)
                return;

            // The teleport cancels any active path without firing OnDestinationReached.
            hasActiveDestination = false;

            // Snap onto the NavMesh when possible so the agent stays valid after teleport.
            Vector3 target = NavMesh.SamplePosition(worldPosition, out NavMeshHit navHit, sampleRadius, agent.areaMask)
                ? navHit.position
                : worldPosition;

            agent.Warp(target);
        }

        /// <inheritdoc />
        public void Pause()
        {
            if (agent != null && agent.isOnNavMesh)
                agent.isStopped = true;
        }

        /// <inheritdoc />
        public void Resume()
        {
            if (agent != null && agent.isOnNavMesh)
                agent.isStopped = false;
        }

        // Toggle helper for quick testing from the component context menu (play mode).
        [ContextMenu("Toggle Movement")]
        private void ToggleMovementContext() => SetMovementEnabled(!movementEnabled);

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (agent == null || !agent.hasPath)
                return;

            // Destination marker
            Gizmos.color = movementEnabled ? Color.green : Color.red;
            Gizmos.DrawWireSphere(agent.destination, 0.2f);

            // Path corners
            Vector3[] corners = agent.path.corners;
            for (int i = 1; i < corners.Length; i++)
                Gizmos.DrawLine(corners[i - 1], corners[i]);
        }
#endif
    }
}
