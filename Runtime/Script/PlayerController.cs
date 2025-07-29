using System;
using MyUnityPackage.Controller;
using UnityEngine;


namespace MyUnityPackage.Controller
{
    
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(PlayerState))]
    [RequireComponent(typeof(PlayerAnimation))]
    [RequireComponent(typeof(PlayerMovementInput))]
    public class PlayerController : MonoBehaviour
    {

        [Header("Components")]

        [SerializeField] private Camera playerCamera;
        private PlayerState playerState;
        private PlayerMovementInput inputManager;
        private CharacterController characterController;

        [Header("Variable")]
        [SerializeField]private float walkSpeed = 2;
        [SerializeField]private float walkAcceleration = 25;
        [SerializeField]private float runSpeed =4;
        [SerializeField]private float runAcceleration=35;
        [SerializeField]private float sprintSpeed =7;
        [SerializeField]private float sprintAcceleration=50;
        [SerializeField]private float inAirAcceleration = 25f;
        
    
        [SerializeField]private float gravity=25;
        [SerializeField]private float jumpForce=1;
        [SerializeField]private float drag = 20;
        [SerializeField]private float inAirDrag;
        [SerializeField]private float antiBump ;

        private bool jumpedLastFrame;
        private float verticalVelocity;
        private float stepOffset;
        [Header("Camera")]
        public float lookSenseHorizontal = 0.1f;
        public float lookSenseVertical = 0.1f;
        public float lookLimitVertical = 89f;

        public float movingThreshold= 0.01f;
        private Vector2 cameraRotation = Vector2.zero;
        private Vector2 playerTargetRot;
  

        [SerializeField] private LayerMask groundLayer; 
        void Awake()
        {
            inputManager = GetComponent<PlayerMovementInput>();
            playerState = GetComponent<PlayerState>();
            characterController = GetComponent<CharacterController>();   
        
            antiBump = sprintSpeed;
            stepOffset = characterController.stepOffset;
        }
        // Update is called once per frame
        void Update()
        {
            UpdateState();
            UpdateMovement();
        }

        void LateUpdate()
        {
            UpdateCamera();
        }

        private void UpdateState()
        {
            bool canRun = CanRun();
            bool isMovementInput = inputManager.MovementInput != Vector2.zero;
            bool isMovingLaterally = IsMovingLaterally();
            bool isSprinting = inputManager.IsSprinting && isMovingLaterally;
            bool isWalking = !canRun && isMovingLaterally ;
            bool isGrounded = IsGrounded();

            EPlayerState lateralState = isWalking ? EPlayerState.Walk :
                                        isSprinting? EPlayerState.Run :
                                        isMovingLaterally || isMovementInput? EPlayerState.Run: EPlayerState.Idle;
            playerState.SetPlayerState(lateralState);

            if((!isGrounded|| jumpedLastFrame) && characterController.velocity.y>0f)
            {
                playerState.SetPlayerState(EPlayerState.Jump);
                jumpedLastFrame = false;
                characterController.stepOffset = 0f;
            }
            else if ((!isGrounded|| jumpedLastFrame) && characterController.velocity.y<=0f)
            {
                playerState.SetPlayerState(EPlayerState.Fall);
                jumpedLastFrame = false;
                characterController.stepOffset = 0f;
            }
            else
            {
                characterController.stepOffset = stepOffset;
            }
                
        }

        private void UpdateMovement()
        {
            //Vertical

            bool isGrounded =  playerState.IsGrounded();

            verticalVelocity -= gravity*Time.deltaTime;
            if(isGrounded && verticalVelocity<0)
                verticalVelocity = -antiBump;
        
            if(inputManager.JumpPressed && isGrounded)
            {
                verticalVelocity += antiBump + (float)Math.Sqrt(jumpForce*3*gravity);
                jumpedLastFrame = true;
            }
            


            //Lateral

            bool isSprinting = playerState.GetPlayerState() == EPlayerState.Sprint;
            bool isWalking = playerState.GetPlayerState() == EPlayerState.Walk;
            float lateralAcceleration = !isGrounded ? inAirAcceleration:
                                isWalking?  walkAcceleration: 
                                isSprinting ? sprintAcceleration : runAcceleration;
            float clampLateralMagnitude = !isGrounded ? sprintSpeed :
                                        isWalking ? walkSpeed : 
                                        isSprinting ? sprintSpeed: runSpeed;

            Vector3 cameraForwardXZ = new Vector3(playerCamera.transform.forward.x,0,playerCamera.transform.forward.z).normalized;
            Vector3 cameraRightXZ =new Vector3(playerCamera.transform.right.x,0,playerCamera.transform.right.z).normalized;
            Vector3 movementDirection = cameraRightXZ * inputManager.MovementInput.x +cameraForwardXZ * inputManager.MovementInput.y;
        
            Vector3 movementDelta = movementDirection * lateralAcceleration * Time.deltaTime;
            Vector3 newVelocity = characterController.velocity + movementDelta;

            // Add drag to player
            float dragMagnitude = isGrounded ? drag : inAirDrag;
            Vector3 currentDrag = newVelocity.normalized * dragMagnitude * Time.deltaTime;
            //Debug.Log("New Veloc" + newVelocity);
            newVelocity = (newVelocity.magnitude > dragMagnitude * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
            newVelocity = Vector3.ClampMagnitude(new Vector3(newVelocity.x, 0f, newVelocity.z), clampLateralMagnitude);
            newVelocity.y += verticalVelocity;
            newVelocity = !isGrounded ? HandleSteepWalls(newVelocity) : newVelocity;
            //Debug.Log("New Veloc" + newVelocity);
            // Move character (Unity suggests only calling this once per tick)
            characterController.Move(newVelocity * Time.deltaTime);
        
        }

        private void UpdateCamera()
        {
            cameraRotation.x += lookSenseHorizontal * inputManager.LookInput.x;
            cameraRotation.y = Mathf.Clamp(cameraRotation.y-lookSenseVertical * inputManager.LookInput.y,-lookLimitVertical,lookLimitVertical);
        
            playerTargetRot.x += transform.eulerAngles.x+lookSenseHorizontal*inputManager.LookInput.x;
            transform.rotation = Quaternion.Euler(0,playerTargetRot.x,0);

            playerCamera.transform.rotation = Quaternion.Euler(cameraRotation.y,cameraRotation.x,0);
        }

        private Vector3 HandleSteepWalls(Vector3 velocity)
        {
            Vector3 normal = CharacterControllerUtils.GetNormalWithSphereCast(characterController,groundLayer);
            float angle = Vector3.Angle(normal,Vector3.up);

            bool validAngle = angle <= characterController.slopeLimit;
            if(!validAngle && verticalVelocity <0f)
            {
                velocity = Vector3.ProjectOnPlane(velocity,normal);
            }

            return velocity;
        }
        private bool IsMovingLaterally()
        {
            Vector3 lateralVelocity = new Vector3(characterController.velocity.x, 0,characterController.velocity.y);

            return lateralVelocity.magnitude > movingThreshold;
        }

        private bool IsGrounded()
        {
            bool grounded = playerState.IsGrounded() ? IsGroundedWhileGrounded() : IsGroundedWhileAirborne();
            return grounded;

        }
        private bool IsGroundedWhileAirborne()
        {
            Vector3 normal = CharacterControllerUtils.GetNormalWithSphereCast(characterController,groundLayer);
            float angle = Vector3.Angle(normal,Vector3.up);
            print("Airborne "  + angle);
            bool validAngle = angle <= characterController.slopeLimit;
            return characterController.isGrounded && validAngle;
        }
        private bool IsGroundedWhileGrounded()
        {
            Vector3 spherePos = new Vector3(transform.position.x,transform.position.y - characterController.radius,transform.position.z);
        
            bool grounded = Physics.CheckSphere(spherePos,characterController.radius,groundLayer,QueryTriggerInteraction.Ignore);
            return grounded;
        }

        private bool CanRun() => inputManager.MovementInput.y >= Mathf.Abs(inputManager.MovementInput.x);

    }

}