using System;
using UnityEngine;

namespace MyUnityPackage.Controller
{
    public interface IPlayerMovement
    {
        public event Action<bool> OnJumpEvent;
        public event Action<Vector2> OnMoveEvent;
        public event Action OnCrouchEvent;
        public event Action<Vector2> OnLookEvent;
        public event Action<bool> OnSprintEvent;
    }

}
