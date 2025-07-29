using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

namespace MyUnityPackage.Controller
{
    [DefaultExecutionOrder(-2)]
    public class ThirdPersonInput : MonoBehaviour,PlayerControls.IThirdPersonActions
    {

        [Header("Components")]
        [SerializeField] private CinemachineCamera virtualCamera;
        [SerializeField] private CinemachineThirdPersonFollow thirdPersonFollow; 
        private PlayerInputManager inputManager;

        [Header("Variable")]
        public Vector2 scrollInput;
        float cameraZoomSpeed =0.1f;
        float cameraMinZoom = 1f;
        float cameraMaxZoom = 5f;
        #region UNITY_FONCTIONS
        private void Awake()
        {
            inputManager = PlayerInputManager.Instance;
        }
        private void OnEnable()
        {

            inputManager.PlayerControls.ThirdPerson.Enable();
            inputManager.PlayerControls.ThirdPerson.SetCallbacks(this);

        }

        private void OnDisable()
        {
            inputManager.PlayerControls.ThirdPerson.Disable();
            inputManager.PlayerControls.ThirdPerson.RemoveCallbacks(this);
        }

       
        private void Update()
        {
            thirdPersonFollow.CameraDistance = Mathf.Clamp(thirdPersonFollow.CameraDistance + scrollInput.y, cameraMinZoom, cameraMaxZoom);
        }

        private void LateUpdate()
        {
            scrollInput = Vector2.zero;
        }
        #endregion

        #region Input Callbacks
        public void OnZoom(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            Vector2 _scrollInput = context.ReadValue<Vector2>();
            scrollInput = -1f * _scrollInput.normalized * cameraZoomSpeed;
        }
        #endregion
        
    }

}
