using UnityEngine;
using UnityEngine.Events;
/*
namespace MyUnityPackage.Controller
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class TwoDController : PersonController
    {
        Rigidbody2D rb;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Init();
        }
        protected override void Init()
        {
            base.Init();
            if(rb == null) 
                rb = GetComponent<Rigidbody2D>();

            InputManager.GetInstance().OnPressDirection += Move;
            InputManager.GetInstance().OnJumpPressed += Jump;

        }
        protected override void Move(Vector2 move)
        {
            Vector2 direction = new Vector3(move.x, move.y);
            //direction = transform.TransformDirection(direction);
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }

        protected override void Jump()
        {
            base.Jump();
            rb.AddForce(new Vector2(0,jumpForce));
        }
    }

}
*/