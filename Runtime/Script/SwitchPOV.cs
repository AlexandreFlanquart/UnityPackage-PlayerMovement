
using UnityEngine;

namespace MyUnityPackage.Controller
{
    public class SwitchPOV : MonoBehaviour
    {
        private IThirdPersoninput thirdPersonInput;
        [SerializeField] private GameObject ThirdCamera;
        [SerializeField] private GameObject FirstCamera;
        [SerializeField] private bool isFirstPerson = false;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            thirdPersonInput = GetComponent<IThirdPersoninput>();
        }
        
        public void Switch()
        {
            isFirstPerson = !isFirstPerson;
           
            MonoBehaviour third = thirdPersonInput as MonoBehaviour;
            third.enabled = !third.enabled ;
            FirstCamera.SetActive(!FirstCamera.activeSelf);
            ThirdCamera.SetActive(!ThirdCamera.activeSelf);
        }
    }
}

