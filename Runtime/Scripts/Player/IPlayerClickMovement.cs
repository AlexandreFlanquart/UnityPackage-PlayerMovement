using System;
using UnityEngine;

namespace MyUnityPackage.Controller
{
    public interface IPlayerClickMovement
    {
        // Screen-space position (pixels)
        event Action<Vector2> OnClickMove;
    }
}
