using MyUnityPackage.Toolkit;
using UnityEngine;
using UnityEngine.AI;

namespace MyUnityPackage.Controller
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class IAController2D : MonoBehaviour, IMovementControl
    {
        public enum NextPointMode { Random, Path }
        public enum PathMode { Loop, PingPong }

        [Header("Main")]
        [SerializeField] private NextPointMode nextPointMode = NextPointMode.Random;
        [Tooltip("Optional collider restricting random patrol points. Assign a scene collider on the placed instance — a prefab asset cannot reference scene objects. Candidates are generated within the random radius around the agent, then rejected if outside this zone.")]
        [SerializeField] private Collider2D patrolZone;

        [Tooltip("Min wait time after reaching a target. Set to 0 for no wait.")]
        [SerializeField] private float waitMin = 0;
        [Tooltip("Max wait time after reaching a target. Set to 0 for no wait.")]
        [SerializeField] private float waitMax = 1f;

        [Header("Random")]
        // How far around the agent we try to pick a new random point
        [SerializeField, Min(0.1f)] private float randomRadiusMax = 6f;
        [SerializeField, Min(0.1f)] private float randomRadiusMin = 1f;

        [Tooltip("Max distance allowed to \"snap\" the random candidate onto the NavMesh")]
        [SerializeField, Min(0.1f)] private float sampleRadius = 1.0f;

        [Tooltip("Number of attempts to find a valid random point each time we need one. More attempts = better chance to find a point, but worse performance.")]
        [SerializeField, Min(1)] private int tries = 100;

        [Header("Path")]
        [SerializeField] private Transform[] points;

        [Tooltip("When using a path, defines how the agent moves through the points.")]
        [SerializeField] private PathMode pathMode = PathMode.Loop;

        // Squared velocity below which the agent is considered idle (~0.01 units/s).
        private const float MovingEpsilonSqr = 0.0001f;

        // Distance below which a destination is considered reached.
        private const float ReachedThreshold = 0.15f;

        private NavMeshAgent _agent;

        private bool _movementEnabled = true;  // when false, patrol is frozen (see IMovementControl)
        private bool _needsDestination;        // deferred: pick a destination once the agent is valid
        private float _waitTimer;      // time left before picking the next destination
        private bool _waiting;         // true when we reached a destination and are waiting

        private int _index = -1;       // current waypoint index
        private int _dir = 1;          // pingpong direction

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();

            // 2D-friendly agent setup (often not exposed in Inspector)
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
            _agent.angularSpeed = 0f;
        }

        private void OnEnable()
        {
            _waiting = false;
            _waitTimer = 0f;
            // The agent may not be on the NavMesh yet; defer the first destination to Update
            // (which is guarded by isOnNavMesh) instead of touching the agent here.
            _needsDestination = true;
        }

        private void Update()
        {
            if (!_agent.isOnNavMesh)
                return;

            // Frozen (e.g. during a dialogue): do not patrol or pick new destinations.
            if (!_movementEnabled)
                return;

            if (_needsDestination)
            {
                AcquireDestination();
                return;
            }

            // If we are waiting, count down and pick when done.
            if (_waiting)
            {
                _waitTimer -= Time.deltaTime;
                if (_waitTimer <= 0f)
                {
                    MUPLogger.Info("Wait over, picking next destination.");
                    _waiting = false;
                    AcquireDestination();
                }
                return;
            }

            if (HasReachedDestination(_agent, ReachedThreshold))
            {
                MUPLogger.Info("Reached destination, starting wait.");
                StartWait();
            }
        }

        bool HasReachedDestination(NavMeshAgent agent, float threshold)
        {
            if (agent.remainingDistance > Mathf.Max(agent.stoppingDistance, threshold))
                return false;

            // Optional but strongly recommended
            if (agent.velocity.sqrMagnitude > 0.01f)
                return false;

            return true;
        }

        private void StartWait()
        {
            MUPLogger.Info("Starting wait timer.");
            _waitTimer = Random.Range(waitMin, waitMax);
            _waiting = true;
        }

        // Picks the next destination and makes sure the agent is allowed to move toward it
        // (clearing a lingering isStopped left by a previous Stop()).
        private void AcquireDestination()
        {
            _needsDestination = false;
            _agent.isStopped = false;
            PickNextDestination();
        }

        private void PickNextDestination()
        {
            if (nextPointMode == NextPointMode.Path)
                PickPathPoint();
            else
                PickRandomPoint();
        }

        private void PickPathPoint()
        {
            if (points == null || points.Length == 0)
            {
                _agent.ResetPath();
                MUPLogger.Warning("No path points assigned for IAController2D.");
                return;
            }

            _index = NextIndex(_index, points.Length, pathMode, ref _dir);

            // Skip null entries (simple safety)
            int safety = 0;
            while (safety < points.Length && points[_index] == null)
            {
                _index = NextIndex(_index, points.Length, pathMode, ref _dir);
                safety++;
            }

            if (points[_index] == null)
            {
                _agent.ResetPath();
                MUPLogger.Warning("All path points are null for IAController2D.");
                return;
            }

            // Try to set this waypoint as destination
            Vector3 target = points[_index].position;
            _agent.SetDestination(target);
        }

        /// <summary>
        /// Pure waypoint index stepper. <paramref name="pointCount"/> &lt;= 0 returns -1; a single
        /// point always returns 0; out-of-range indices are clamped back into range. A seed of -1
        /// yields the first waypoint (index 0) for both modes.
        /// </summary>
        internal static int NextIndex(int currentIndex, int pointCount, PathMode mode, ref int pingPongDirection)
        {
            if (pointCount <= 0)
                return -1;
            if (pointCount == 1)
                return 0;

            if (mode == PathMode.PingPong)
            {
                int next = currentIndex + pingPongDirection;
                if (next >= pointCount)
                {
                    next = pointCount - 2;
                    pingPongDirection = -1;
                }
                else if (next < 0)
                {
                    next = 1;
                    pingPongDirection = 1;
                }
                return next;
            }

            // Loop
            int incremented = currentIndex + 1;
            return (incremented >= pointCount || incremented < 0) ? 0 : incremented;
        }

        private void PickRandomPoint()
        {
            Vector3 origin = transform.position;
            int mask = _agent.areaMask;

            for (int i = 0; i < tries; i++)
            {
                // Candidate = a potential destination (not validated yet)
                Vector2 rnd = Random.insideUnitCircle * Random.Range(randomRadiusMin, randomRadiusMax);
                Vector3 candidate = new Vector3(origin.x + rnd.x, origin.y + rnd.y, origin.z);

                // Snap candidate onto NavMesh within sampleRadius
                if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, sampleRadius, mask))
                    continue;

                if (patrolZone != null && !patrolZone.OverlapPoint((Vector2)hit.position))
                    continue;

                // Reject points too close to the NavMesh border
                if (NavMesh.FindClosestEdge(hit.position, out NavMeshHit edgeHit, mask))
                {
                    if (edgeHit.distance < _agent.radius )
                    {
                        //MUPLogger.Warning($"Rejected candidate at {hit.position} for being too close to edge ({edgeHit.distance}m).", this);
                        continue;
                    }
                }

                if (_agent.SetDestination(hit.position))
                    return;
            }
            MUPLogger.Warning("Failed to find a valid random destination for IAController2D.");
            _agent.ResetPath();
        }

        // ---- IMovementControl -----------------------------------------

        /// <inheritdoc />
        public bool MovementEnabled => _movementEnabled;

        /// <inheritdoc />
        public bool IsMoving => _agent != null && _agent.hasPath && _agent.velocity.sqrMagnitude > MovingEpsilonSqr;

        /// <inheritdoc />
        public void SetMovementEnabled(bool enabled)
        {
            if (_movementEnabled == enabled)
                return;

            _movementEnabled = enabled;

            if (!enabled)
                Stop();
            else
                // Resume patrolling; deferred so it works even if the agent is briefly off-mesh.
                _needsDestination = true;
        }

        /// <summary>
        /// Interrupts the current path immediately. While movement stays enabled the agent
        /// resumes patrol after its normal wait; use <see cref="SetMovementEnabled"/> with
        /// <c>false</c> for a durable freeze.
        /// </summary>
        public void Stop()
        {
            _waiting = false;
            _waitTimer = 0f;
            _needsDestination = false;

            if (_agent == null || !_agent.isOnNavMesh)
                return;

            _agent.isStopped = true;
            _agent.ResetPath();
        }

    }
}
