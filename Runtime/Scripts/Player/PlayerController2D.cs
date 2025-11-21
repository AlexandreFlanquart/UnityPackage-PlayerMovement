using MyUnityPackage.Toolkit;
using UnityEngine;

namespace MyUnityPackage.Controller
{
    
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController2D : MonoBehaviour
    {   
        [SerializeField] public float walkSpeedMax = 2;
        [SerializeField] public float sprintSpeedMax = 4;
        [SerializeField] public float acceleration = 50;
        [SerializeField] public float drag = 3f;

        private  IPlayerMovement inputManager;
        private Rigidbody2D rb;
        public float currentSpeedMax;
        private bool canMove = true;

        private void Awake() {
            inputManager = GetComponent<IPlayerMovement>();
        
            rb = GetComponent<Rigidbody2D>();
            rb.linearDamping = drag;

            currentSpeedMax = walkSpeedMax;

            inputManager.OnMoveEvent += OnPlayerMove;
        }

        public void OnPlayerMove(Vector2 p_movementInput) {
            if(!canMove) return;
            MUPLogger.Info("OnPlayerMove : " + p_movementInput);
            if (p_movementInput != Vector2.zero)
            {
                Move(p_movementInput * acceleration);

            }
            // Limit speed
            if (rb.linearVelocity.sqrMagnitude  > currentSpeedMax*currentSpeedMax)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * currentSpeedMax;
            }
            
        }

        public void Move(Vector2 force){
            MUPLogger.Info("Move : " + force);
            rb.AddForce(force);
        }

        public void EnableMovement(){
            canMove = true;
        }

        public void DesableMovement(){
            rb.linearVelocity = Vector2.zero;
            canMove = false;
        }

        public void Teleport(Vector3 position){
            rb.linearVelocity = Vector2.zero;
            rb.position = position;
        }


        private void Update(){
            /*
            if(oldSpeed != rb.linearVelocity.magnitude){

                OnChangeMovementInput?.Invoke(rb.linearVelocity);
                oldSpeed = rb.linearVelocity.magnitude;
            }
            */
        }
    }
}