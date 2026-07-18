using System;
using System.Collections;
using MyUnityPackage.Toolkit;
using UnityEngine;
using UnityEngine.AI;

namespace MyUnityPackage.Controller
{
    /// <summary>
    /// Simple 2D vision cone rendered as a triangle fan mesh.
    /// The cone is clipped by obstacles using raycasts.
    /// Also supports basic target detection (target blocks the ray like a wall, but counts as "seen").
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [DisallowMultipleComponent]
    public sealed class VisionCone2D : MonoBehaviour
    {
        [Header("Cone")]
        [SerializeField, Min(0.1f)] private float distance = 6f;
        [SerializeField, Range(1f, 360f)] private float angle = 90f;
        [Tooltip("Number of triangles in the cone mesh. More triangles = smoother cone and more precise vision, but more raycasts and worse performance.")]
        [SerializeField, Min(3)] private int triangles = 30; 
        [Tooltip("Delay between raycasts in seconds. Longer delay = better performance but less responsive vision. Cached when the component is enabled; disable/enable to apply runtime changes.")]
        [SerializeField] private float delayBetweenRays = 0.1f;

        [Header("Masks")]
        [Tooltip("Layers of obstacles that block vision (e.g. walls). They will block vision, but not set TargetVisible = true.")]
        [SerializeField] private LayerMask obstacleMask;
        [Tooltip("Layers of objects that can be seen (e.g. player). They will block vision like obstacles, but also set TargetVisible = true.")]
        [SerializeField] private LayerMask targetMask;  

        [Header("Facing (optional)")]
        [Tooltip("Optional NavMeshAgent to derive facing direction from its desired velocity. If null, uses local right as facing.")]
        [SerializeField] private NavMeshAgent agent;     
        [Tooltip("Ray origin (eyes). If null, uses this transform")]
        [SerializeField] private Transform origin;

        // Public read-only: was target hit by any ray this frame?
        public bool TargetVisible { get; private set; }
        public Transform LastSeenTarget { get; private set; }

        /// <summary>Raised once when a target becomes visible (TargetVisible false→true). Passes the seen transform.</summary>
        public event Action<Transform> OnTargetSpotted;

        /// <summary>Raised once when the target stops being visible (true→false), including when the cone is disabled while a target was visible.</summary>
        public event Action OnTargetLost;

        private MeshFilter _meshFilter;
        private Mesh _mesh;

        // Cached facing direction (world space), used to rotate the cone without rotating the agent.
        private Vector2 _facing = Vector2.right;

        // Buffers to avoid allocations every frame.
        private Vector3[] _vertices;
        private int[] _indices;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _mesh = new Mesh { name = "VisionCone2D" };
            _meshFilter.mesh = _mesh;

            if (!origin) origin = transform;
        }

        private void OnEnable()
        {
            StartCoroutine(UpdatingVision());
        }

        private void OnDisable()
        {
            StopAllCoroutines();

            // Don't leave listeners believing a target is still visible.
            if (TargetVisible)
            {
                TargetVisible = false;
                LastSeenTarget = null;
                OnTargetLost?.Invoke();
            }
        }

       IEnumerator UpdatingVision()
        {
            // Cached once per enable to avoid a per-iteration allocation.
            WaitForSeconds wait = new WaitForSeconds(delayBetweenRays);

            while (true)
            {
                bool wasVisible = TargetVisible;

                UpdateFacing();
                BuildMesh();

                // Edge-triggered detection events (no per-frame spam).
                if (TargetVisible != wasVisible)
                {
                    if (TargetVisible)
                    {
                        MUPLogger.Info("VisionCone2D: target spotted.", this);
                        OnTargetSpotted?.Invoke(LastSeenTarget);
                    }
                    else
                    {
                        MUPLogger.Info("VisionCone2D: target lost.", this);
                        OnTargetLost?.Invoke();
                    }
                }

                yield return wait;
            }
        }

        private void UpdateFacing()
        {
            // 2D NavMesh: agent doesn't rotate, so we derive a facing direction from its desired velocity.
            if (agent != null)
            {
                Vector3 v = agent.desiredVelocity;             
                Vector2 move = new Vector2(v.x, v.y);

                // Keep last direction if almost stopped (avoids jitter)
                if (move.sqrMagnitude > 0.0001f)
                    _facing = move.normalized;

                return;
            }

            // Fallback if no agent provided: keep current local right as facing.
            _facing = transform.right;
        }

       private void BuildMesh()
        {
            TargetVisible = false;
            LastSeenTarget = null;

            int rayCount = Mathf.Max(2, triangles + 1);
            EnsureBuffers(rayCount);

            // Build everything in LOCAL space
            Vector3 originWorld = origin ? origin.position : transform.position;
            Vector3 originLocal = transform.InverseTransformPoint(originWorld);

            // Vertex 0 = center of the fan
            _vertices[0] = new Vector3(originLocal.x, originLocal.y, 0f);

            float half = angle * 0.5f;
            float step = angle / (rayCount - 1);

            int rayMask = obstacleMask | targetMask;

            for (int i = 0; i < rayCount; i++)
            {
                float a = -half + step * i;
                Vector2 dir = Rotate2D(_facing, a);

                RaycastHit2D hit = Physics2D.Raycast(originWorld, dir, distance, rayMask);

                Vector3 pointWorld;
                if (hit.collider != null)
                {
                    pointWorld = hit.point;

                    if (IsInMask(hit.collider.gameObject.layer, targetMask))
                    {
                        TargetVisible = true;
                        LastSeenTarget = hit.collider.transform;
                    }
                }
                else
                {
                    pointWorld = originWorld + (Vector3)(dir * distance);
                }

                Vector3 pointLocal = transform.InverseTransformPoint(pointWorld);
                _vertices[i + 1] = new Vector3(pointLocal.x, pointLocal.y, 0f); // force same plane
            }

            int idx = 0;
            for (int i = 0; i < rayCount - 1; i++)
            {
                _indices[idx++] = 0;
                _indices[idx++] = i + 2;
                _indices[idx++] = i + 1;
            }

            _mesh.Clear(false);
            _mesh.vertices = _vertices;
            _mesh.triangles = _indices;
            _mesh.RecalculateBounds();
        }

        private void EnsureBuffers(int rayCount)
        {
            int vCount = rayCount + 1;
            int iCount = (rayCount - 1) * 3;

            if (_vertices == null || _vertices.Length != vCount)
                _vertices = new Vector3[vCount];

            if (_indices == null || _indices.Length != iCount)
                _indices = new int[iCount];
        }

        private static Vector2 Rotate2D(Vector2 v, float degrees)
        {
            float rad = degrees * Mathf.Deg2Rad;
            float c = Mathf.Cos(rad);
            float s = Mathf.Sin(rad);
            return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
        }

        private static bool IsInMask(int layer, LayerMask mask)
        {
            return (mask.value & (1 << layer)) != 0;
        }
    }
}
