using MyUnityPackage.Toolkit;
using UnityEngine;
using UnityEngine.AI;

namespace MyUnityPackage.Controller
{
    /// <summary>
    /// Drives a 2D Animator from a NavMeshAgent's desired velocity.
    /// Mirrors PlayerAnimator2D but reads NavMeshAgent.desiredVelocity instead of Rigidbody2D.linearVelocity.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class IAAnimator2D : MonoBehaviour
    {
        [Header("Animator parameter names")]
        [SerializeField] private string moveXParam = "MoveX";
        [SerializeField] private string moveYParam = "MoveY";
        [SerializeField] private string speedParam = "Speed";

        private Animator _animator;
        private NavMeshAgent _agent;
        private Vector2 _lastMoveDirection = Vector2.down;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            Vector2 velocity = new Vector2(_agent.desiredVelocity.x, _agent.desiredVelocity.y);
            float speed = velocity.magnitude;

            if (speed > 0.01f)
                _lastMoveDirection = velocity.normalized;

            _animator.SetFloat(moveXParam, _lastMoveDirection.x);
            _animator.SetFloat(moveYParam, _lastMoveDirection.y);
            _animator.SetFloat(speedParam, speed);
        }
    }
}
