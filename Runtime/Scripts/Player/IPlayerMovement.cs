using System;
using UnityEngine;

namespace MyUnityPackage.Controller
{
    public interface IPlayerMovement
    {
        /*
            public Vector2 MovementInput { get; set; }
            public Vector2 LookInput { get; set; }
            public bool IsCrounching { get; set; }
            public bool JumpPressed { get; set; }
            public bool IsSprinting { get; set; }
        */

        public event Action<bool> OnJumpEvent;
        public event Action<Vector2> OnMoveEvent;
        public event Action OnCrouchEvent;
        public event Action<Vector2> OnLookEvent;
        public event Action<bool> OnSprintEvent;
    }

}
