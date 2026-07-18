namespace MyUnityPackage.Controller
{
    /// <summary>
    /// Minimal runtime control contract shared by every player/agent controller in the
    /// package (2D click-to-move, 2D Rigidbody player, 2D NavMesh AI). Lets game code
    /// freeze/restore a mover — e.g. to block movement during a dialogue — without knowing
    /// the concrete controller or reaching into its Rigidbody/NavMeshAgent.
    /// Resolve it with <c>GetComponent&lt;IMovementControl&gt;()</c>.
    /// </summary>
    public interface IMovementControl
    {
        /// <summary>True while input/AI is allowed to drive this mover.</summary>
        bool MovementEnabled { get; }

        /// <summary>True while the mover is actually moving.</summary>
        bool IsMoving { get; }

        /// <summary>
        /// Enables or disables movement. Passing <c>false</c> also stops the mover
        /// immediately so it freezes in place (see <see cref="Stop"/>); passing
        /// <c>true</c> restores normal piloting. Idempotent.
        /// </summary>
        /// <param name="enabled">Whether movement should be driven.</param>
        void SetMovementEnabled(bool enabled);

        /// <summary>Stops the mover immediately.</summary>
        void Stop();
    }
}
