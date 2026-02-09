using MyUnityPackage.Toolkit;
using UnityEngine;
using UnityEngine.AI;

namespace MyUnityPackage.Controller
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class IAController2D : MonoBehaviour
    {
        public enum NextPointMode { Random, Path }
        public enum PathMode { Loop, PingPong }

        [Header("Main")]
        [SerializeField] private NextPointMode nextPointMode = NextPointMode.Random;
        [Tooltip("Optional collider defining the patrol area. If assigned, random points will be generated within its bounds.")]
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

        private NavMeshAgent _agent;

        private float _waitTimer;      // time left before picking the next destination
        private bool _waiting;         // true when we reached a destination and are waiting

        private int _index = -1;            // current waypoint index
        private int _dir = 1;          // pingpong direction
        private Vector3 _lastPosition;    // last frame position
        //private float stuckTimer = 0f;

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
            _lastPosition = transform.position;
            PickNextDestination();
        }

        private void Update()
        {
            if (!_agent.isOnNavMesh)
                return;

            // If we are waiting, count down and pick when done.
            if (_waiting)
            {
                _waitTimer -= Time.deltaTime;
                if (_waitTimer <= 0f)
                {
                    MUPLogger.Info("Wait over, picking next destination.");
                    _waiting = false;
                    PickNextDestination();
                }
                return;
            }
      
            if (HasReachedDestination(_agent, 0.15f))
            {
                MUPLogger.Info("Reached destination, starting wait.");
                StartWait();
            }
            else
            {/*
                // If agent has a path but barely moves for a while, consider it stuck.
                if (_agent.hasPath && !_agent.pathPending)
                {
                    float moved = (transform.position - _lastPosition).sqrMagnitude;

                    if (moved < 0.0009f) stuckTimer += Time.deltaTime;
                    else stuckTimer = 0f;
                    MUPLogger.Info($"Agent moved {moved}, stuck timer at {stuckTimer}s.", this);
                    _lastPosition = transform.position;

                    if (stuckTimer > 2f)
                    {
                        stuckTimer = 0f;
                        PickNextDestination(); 
                        MUPLogger.Warning("Agent was stuck, picking new destination.", this);
                    }
                }
                */
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

            GetNextIndex();

            // Skip null entries (simple safety)
            int safety = 0;
            while (safety < points.Length && points[_index] == null)
            {
                GetNextIndex();
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

        private void GetNextIndex()
        {
            if (points.Length <= 1) return;
            if (pathMode == PathMode.PingPong)
            {
                _index += _dir;

                if (_index >= points.Length)
                {
                    _index = points.Length - 2;
                    _dir = -1;
                }
                else if (_index < 0)
                {
                    _index = 1;
                    _dir = 1;
                }
            }
            else // Loop
            {
                _index++;
                if (_index >= points.Length)
                    _index = 0;
            }
            MUPLogger.Info($"New path index is {_index}.");
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

    }
}
