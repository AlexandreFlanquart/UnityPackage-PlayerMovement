using UnityEngine;

namespace MyUnityPackage.Controller
{
    public interface IThirdPersoninput
    {
        public Vector2 ScrollInput {get;set;}
        public float CameraZoomSpeed {get;set;}
        public float CameraMinZoom  {get;set;}
        public float CameraMaxZoom {get;set;}
    }
}

