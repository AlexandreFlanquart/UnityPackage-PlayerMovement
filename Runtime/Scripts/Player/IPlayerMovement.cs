using System;
using UnityEngine;

namespace MyUnityPackage.Controller
{
    public interface IPlayerMovement
    {
        // Jump events
        public event Action OnJumpPressed;
        public event Action OnJumpReleased;

        // Movement input (WASD / left stick)
        public event Action<Vector2> OnMoveEvent;

        // Look input (mouse delta / right stick)
        public event Action<Vector2> OnLookEvent;

        // Crouch events
        public event Action OnCrouchStarted;
        public event Action OnCrouchCanceled;

        // Sprint events
        public event Action OnSprintStarted;
        public event Action OnSprintCanceled;

        // Click event (for click-to-move systems)
        public event Action<Vector2> OnClickPressed;
    }

}
