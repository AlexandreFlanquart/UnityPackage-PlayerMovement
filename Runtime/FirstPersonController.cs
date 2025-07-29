using UnityEngine;
using UnityEngine.Events;

/*
namespace MyUnityPackage.Controller
{
     [RequireComponent(typeof(Rigidbody))]
    public class FirstPersonController : PersonController
    {
        Rigidbody rb;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
           Init();
        }

        protected override void Init()
        {
            base.Init();
            if(rb == null) 
                rb = GetComponent<Rigidbody>();

            if(InputManager.GetInstance() == null)
                throw new System.Exception("InputManager not found");
            InputManager.GetInstance().OnPressDirection += Move;
            InputManager.GetInstance().OnLookDirection += LookAround;
            InputManager.GetInstance().OnJumpPressed += Jump;


        }
        protected override void LookAround(Vector2 look)
        {
            yaw += look.x * lookSensitivity;
            pitch -= look.y * lookSensitivity;
            pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

            // Rotation horizontale du joueur (yaw)
            transform.rotation = Quaternion.Euler(0, yaw, 0);
            // Rotation verticale de la caméra (pitch)
            if (cameraHolder != null)
                cameraHolder.localRotation = Quaternion.Euler(pitch, 0, 0);
        }

        protected override void Move(Vector2 move)
        {
            Vector3 direction = new Vector3(move.x, 0, move.y);
            direction = transform.TransformDirection(direction);
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
        protected override void Jump()
        {
            base.Jump();
            rb.AddForce(new Vector3(0,jumpForce,0));
        }
    }
}
*/
