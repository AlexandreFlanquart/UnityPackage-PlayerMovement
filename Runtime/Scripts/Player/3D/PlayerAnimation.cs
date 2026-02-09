using MyUnityPackage.Controller;
using MyUnityPackage.Toolkit;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;


namespace MyUnityPackage.Controller
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField]private float blendSpeed = 0.02f;

        private IPlayerMovement inputManager;
        private PlayerState playerState;

        [Header("Animator Hash")]
        private static int inputXHash = Animator.StringToHash("InputX");
        private static int inputYHash = Animator.StringToHash("InputY");
        private static int inputMagnitudeHash = Animator.StringToHash("InputMagnitude");

        private static int isGroundedHash = Animator.StringToHash("isGrounded");
        private static int isFallingHash = Animator.StringToHash("isFalling");
        private static int isJumpingHash = Animator.StringToHash("isJumping");
        [Header("Value")]
        private Vector3 currentValue;
        Vector2 moveInputValue;

        [Header("State")]
        bool isJumping;
        bool isGrounded;
        bool isFalling;
        bool isSprinting;
        void Awake()
        {
            playerState = GetComponent<PlayerState>();
            animator = GetComponent<Animator>();
            inputManager = GetComponent<IPlayerMovement>();

            inputManager.OnMoveEvent += Move;
            playerState.OnStateChanged += OnStateChanged;
            
        }
        void Move(Vector2 value)
        {
            moveInputValue = value;
        }
        void Update()
        {
            UpdateState();
        }

        void OnStateChanged()
        {
            MUPLogger.Info("Change state in animation");

            isSprinting = playerState.GetPlayerState() == EPlayerState.Sprint;
            isJumping = playerState.GetPlayerState() == EPlayerState.Jump;
            isFalling = playerState.GetPlayerState() == EPlayerState.Fall;
            isGrounded = playerState.IsGrounded();

            
            animator.SetBool(isGroundedHash, isGrounded);
            animator.SetBool(isFallingHash,isFalling);
            animator.SetBool(isJumpingHash,isJumping);

        }
        private void UpdateState()
        {


            Vector2 input = isSprinting ? moveInputValue * 1.5f : moveInputValue;
            currentValue = Vector3.Lerp(currentValue, input, blendSpeed);

            animator.SetFloat(inputXHash, input.x);
            animator.SetFloat(inputYHash, input.y);
            animator.SetFloat(inputMagnitudeHash, currentValue.magnitude);
        }
    }

}
