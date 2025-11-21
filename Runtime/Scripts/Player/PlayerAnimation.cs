using MyUnityPackage.Controller;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;


namespace MyUnityPackage.Controller
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField]private float blendSpeed = 0.02f;

        private IPlayerMovement/*PlayerMovementInput*/ inputManager;
        private PlayerState playerState;

        private static int inputXHash = Animator.StringToHash("InputX");
        private static int inputYHash = Animator.StringToHash("InputY");
        private static int inputMagnitudeHash = Animator.StringToHash("InputMagnitude");

        private static int isGroundedHash = Animator.StringToHash("isGrounded");
        private static int isFallingHash = Animator.StringToHash("isFalling");
        private static int isJumpingHash = Animator.StringToHash("isJumping");

        private Vector3 currentValue;

        Vector2 moveInputValue;
        void Awake()
        {
            playerState = GetComponent<PlayerState>();
            animator = GetComponent<Animator>();
            inputManager = GetComponent<IPlayerMovement/*PlayerMovementInput*/>();

    
        }
        void Move(Vector2 value)
        {
            moveInputValue = value;
        }
        void Update()
        {
            UpdateState();
        }

        private void UpdateState()
        {
            bool isIdling = playerState.GetPlayerState() == EPlayerState.Idle;
            bool isRunning = playerState.GetPlayerState() == EPlayerState.Run;
            bool isSprinting = playerState.GetPlayerState() == EPlayerState.Sprint;
            bool isJumping = playerState.GetPlayerState() == EPlayerState.Jump;
            bool isFalling = playerState.GetPlayerState() == EPlayerState.Fall;
            bool isGrounded = playerState.IsGrounded();

            Vector2 input = isSprinting ? /*inputManager.MovementInput*/moveInputValue*1.5f:/*inputManager.MovementInput*/moveInputValue;
            currentValue = Vector3.Lerp(currentValue, input, blendSpeed);

            animator.SetBool(isGroundedHash, isGrounded);
            animator.SetBool(isFallingHash,isFalling);
            animator.SetBool(isJumpingHash,isJumping);
            animator.SetFloat(inputXHash, input.x);
            animator.SetFloat(inputYHash, input.y);
            animator.SetFloat(inputMagnitudeHash, currentValue.magnitude);
        }
    }

}
