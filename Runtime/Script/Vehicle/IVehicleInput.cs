
using System;
using UnityEngine;

namespace MyUnityPackage.Controller
{
    public interface IVehicleInput 
    {
        public Vector2 MovementInput { get; set; }
        public bool IsDrifting { get; set; }

        public Action<bool> OnKlaxonAction { get; set;}
        public Action<bool> OnDriftAction { get; set;}
        public void EnableVehicleInput();
        public void DisableVehicleInput();
    }
}

