using System;
using UnityEngine;

namespace MyUnityPackage.Controller
{
    /// <summary>
    /// Runtime control contract for a 2D NavMesh-driven player, independent of the input
    /// source. Extends <see cref="IMovementControl"/> with NavMesh-specific commands
    /// (move/teleport by code) and a resumable pause. Lets game code fully pilot the player
    /// without reaching into the underlying <see cref="UnityEngine.AI.NavMeshAgent"/>.
    /// Resolve it with <c>GetComponent&lt;IPlayerController2D&gt;()</c>.
    /// </summary>
    public interface IPlayerController2D : IMovementControl
    {
        /// <summary>Raised once when a commanded or clicked path completes naturally.</summary>
        event Action OnDestinationReached;

        /// <summary>
        /// Moves the player to a world position by code, ignoring <see cref="IMovementControl.MovementEnabled"/>.
        /// The target is snapped onto the NavMesh before being used as the destination.
        /// </summary>
        /// <param name="worldPosition">Desired world-space destination.</param>
        void MoveTo(Vector3 worldPosition);

        /// <summary>
        /// Instantly repositions (teleports) the player onto the NavMesh at the given world position.
        /// </summary>
        /// <param name="worldPosition">World-space position to teleport to.</param>
        void Warp(Vector3 worldPosition);

        /// <summary>
        /// Pauses movement while keeping the current destination, so <see cref="Resume"/>
        /// continues toward the same point. Unlike <see cref="IMovementControl.Stop"/>, the
        /// path is not cleared. Idempotent.
        /// </summary>
        void Pause();

        /// <summary>Resumes movement toward the destination kept by <see cref="Pause"/>. Idempotent.</summary>
        void Resume();
    }
}
