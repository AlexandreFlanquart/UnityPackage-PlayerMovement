using UnityEngine;

namespace MyUnityPackage.Controller
{
    
    public static class CharacterControllerUtils 
    {
        public static Vector3 GetNormalWithSphereCast(CharacterController charController,LayerMask mask = default)
        {
            Vector3 normal = Vector3.up;
            Vector3 center = charController.transform.position + charController.center;
            float dist = charController.height /2f + charController.stepOffset;


            RaycastHit hit;
            if(Physics.SphereCast(center,charController.radius,Vector3.down,out hit,dist,mask))
            {
                normal = hit.normal;
            }

            return normal;
        }
    }
}
