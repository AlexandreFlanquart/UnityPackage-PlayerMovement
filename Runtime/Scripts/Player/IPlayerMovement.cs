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

        public event Action OnJumpEvent;
        public event Action OnMoveEvent;
        public event Action OnCrouchEvent;
        public event Action OnLookEvent;
        public event Action OnSprintEvent;
    }

}
