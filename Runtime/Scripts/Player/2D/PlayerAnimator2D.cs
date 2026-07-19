using MyUnityPackage.Toolkit;
using UnityEngine;

namespace MyUnityPackage.Controller
{
    /// <summary>
    /// Drives the 2D player Animator from a <see cref="Rigidbody2D"/>'s velocity, and exposes
    /// the sprint/crouch state to the animator. Direction snapping is handled by the base
    /// <see cref="DirectionalAnimator2D"/>.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimator2D : DirectionalAnimator2D
    {
        [Header("References")]
        [SerializeField] private PlayerController2D controller;
        [SerializeField] private Rigidbody2D rb;

        [Header("State parameter names")]
        [SerializeField] private string isSprintingParam = "IsSprinting";
        [SerializeField] private string isCrouchingParam = "IsCrouching";

        private int isSprintingHash;
        private int isCrouchingHash;

        protected override void Awake()
        {
            base.Awake();

            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
                if (rb == null)
                    MUPLogger.Error("PlayerAnimator2D: no Rigidbody2D assigned and none found on this GameObject.", this);
            }

            if (controller == null)
            {
                controller = GetComponent<PlayerController2D>();
                if (controller == null)
                    MUPLogger.Error("PlayerAnimator2D: no PlayerController2D assigned and none found on this GameObject.", this);
            }

            isSprintingHash = UnityEngine.Animator.StringToHash(isSprintingParam);
            isCrouchingHash = UnityEngine.Animator.StringToHash(isCrouchingParam);
        }

        // Another animator source may take over the shared Animator (e.g. a movement-mode
        // switch): these bools are owned by this component, don't leave a stale pose behind.
        private void OnDisable()
        {
            if (Animator != null)
            {
                Animator.SetBool(isSprintingHash, false);
                Animator.SetBool(isCrouchingHash, false);
            }
        }

        protected override Vector2 ReadVelocity()
        {
            return rb != null ? rb.linearVelocity : Vector2.zero;
        }

        protected override void ApplyExtraParameters()
        {
            if (controller == null)
                return;

            Animator.SetBool(isSprintingHash, controller.IsSprintingPublic);
            Animator.SetBool(isCrouchingHash, controller.IsCrouchingPublic);
        }
    }
}
