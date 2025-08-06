using UnityEngine;
using UnityEngine.InputSystem;



namespace MyUnityPackage.Controller
{
    [DefaultExecutionOrder(-3)]
    public class PlayerInputManager : MonoBehaviour
    {
        public PlayerControls PlayerControls {get ; private set;}
        public static PlayerInputManager Instance { get; private set; }
        void Awake()
        {
            Instance = this;
            PlayerControls = new PlayerControls();   
        }

        void OnEnable()
        {
            PlayerControls.Enable();
        }
        void OnDisable()
        {
            PlayerControls.Disable();
        }
    }
}
