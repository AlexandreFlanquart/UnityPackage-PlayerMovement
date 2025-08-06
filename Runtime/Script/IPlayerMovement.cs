using UnityEngine;

namespace MyUnityPackage.Controller
{
    public interface IPlayerMovement
    {
            public Vector2 MovementInput { get; set; }
            public Vector2 LookInput { get; set; }
            public bool IsCrounching { get; set; }
            public bool JumpPressed { get; set; }
            public bool IsSprinting { get; set; }
    }

}
