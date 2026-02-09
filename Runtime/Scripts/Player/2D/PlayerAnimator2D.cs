using MyUnityPackage.Toolkit;
using UnityEngine;

namespace MyUnityPackage.Controller
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimator2D : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController2D controller;
        [SerializeField] private Rigidbody2D rb;

        [Header("Animator parameter names")]
        [SerializeField] private string moveXParam = "MoveX";
        [SerializeField] private string moveYParam = "MoveY";
        [SerializeField] private string speedParam = "Speed";
        [SerializeField] private string isSprintingParam = "IsSprinting";
        [SerializeField] private string isCrouchingParam = "IsCrouching";

        private Animator animator;
        private Vector2 lastMoveDirection = Vector2.down; // default facing down

        private void Awake()
        {
            animator = GetComponent<Animator>();

            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
                MUPLogger.Error("PlayerAnimator2D: No Rigidbody2D reference found.", this);
            }

            if (controller == null)
            {
                controller = GetComponent<PlayerController2D>();
                MUPLogger.Error("PlayerAnimator2D: No PlayerController2D reference found.", this);
            }

        }

        private void Update()
        {
            UpdateMovementAnimation();
            UpdateStateAnimation();
        }

        private void UpdateMovementAnimation()
        {
            // Use velocity to drive movement animation.
            Vector2 velocity = rb != null ? rb.linearVelocity : Vector2.zero;
            float speed = velocity.magnitude;

            // If the player is moving, update facing direction
            if (speed > 0.01f)
            {
                lastMoveDirection = velocity.normalized;
            }

            // Set directional parameters (for 2D blend tree)
            animator.SetFloat(moveXParam, lastMoveDirection.x);
            animator.SetFloat(moveYParam, lastMoveDirection.y);
            animator.SetFloat(speedParam, speed);
        }

        private void UpdateStateAnimation()
        {
            if (controller == null)
                return;

            animator.SetBool(isSprintingParam, controller.IsSprintingPublic);
            animator.SetBool(isCrouchingParam, controller.IsCrouchingPublic);
        }
    }
}
