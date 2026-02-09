using UnityEngine;
using UnityEngine.AI;

namespace MyUnityPackage.Controller
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class ClickToMoveController2D : MonoBehaviour
    {
        [Tooltip("2D ground layers used for the click raycast")]
        [SerializeField] private LayerMask groundMask = ~0; 
        [Tooltip("Max distance allowed to \"snap\" the click point onto the NavMesh")]
        [SerializeField] private float sampleRadius = 1.0f; 
 
        private IPlayerMovement input;
        private NavMeshAgent agent;
        private Camera mainCamera;
        
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
                Debug.LogError("Input source does not implement IPlayerInput.");
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


        private void handleClickInput(Vector2 screenPos)
        {
            if (mainCamera == null)
                return;
            
            // Convert the screen position into a world-space ray
            Ray ray = mainCamera.ScreenPointToRay(screenPos);
            RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray, 100, groundMask);

            // Snap the hit point onto the NavMesh and move the agent
            if (NavMesh.SamplePosition(hit2D.point, out NavMeshHit navHit, sampleRadius, agent.areaMask))
            {
                agent.isStopped = false;
                agent.SetDestination(navHit.position);
            }
        }

    }
}
