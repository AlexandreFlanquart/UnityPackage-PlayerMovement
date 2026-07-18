using UnityEngine;
using UnityEngine.AI;

namespace MyUnityPackage.Controller
{
    /// <summary>
    /// Drives a 2D Animator from a <see cref="NavMeshAgent"/>'s desired velocity. Same behaviour
    /// as <see cref="PlayerAnimator2D"/> but reads the agent instead of a Rigidbody2D. Direction
    /// snapping is handled by the base <see cref="DirectionalAnimator2D"/>.
    /// The agent may live on this GameObject or on a parent (visual-child setups where the root
    /// carries the NavMeshAgent at the feet and the sprite/Animator sit on an offset child).
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class IAAnimator2D : DirectionalAnimator2D
    {
        private NavMeshAgent agent;

        protected override void Awake()
        {
            base.Awake();
            agent = GetComponentInParent<NavMeshAgent>();
        }

        protected override Vector2 ReadVelocity()
        {
            if (agent == null)
                return Vector2.zero;

            Vector3 desired = agent.desiredVelocity;
            return new Vector2(desired.x, desired.y);
        }
    }
}
