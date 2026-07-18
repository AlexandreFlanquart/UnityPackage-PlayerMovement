using NUnit.Framework;
using UnityEngine;

namespace MyUnityPackage.Controller.EditorTests
{
    /// <summary>
    /// Edit-mode tests for the runtime control API. These validate the enable/disable gate
    /// and its idempotence without a baked NavMesh: in edit mode Awake does not run, so the
    /// cached NavMeshAgent is null and the guarded commands must be no-ops rather than throw.
    /// Full path-following behaviour is validated in play mode with a baked NavMesh.
    /// </summary>
    public class MovementControlTests
    {
        private GameObject _go;

        [TearDown]
        public void TearDown()
        {
            if (_go != null)
                Object.DestroyImmediate(_go);
        }

        [Test]
        public void ClickToMove_MovementEnabled_DefaultsTrue_AndNotMoving()
        {
            IPlayerController2D controller = NewClickToMove();

            Assert.IsTrue(controller.MovementEnabled);
            Assert.IsFalse(controller.IsMoving);
        }

        [Test]
        public void ClickToMove_SetMovementEnabled_TogglesAndIsIdempotent()
        {
            IPlayerController2D controller = NewClickToMove();

            controller.SetMovementEnabled(false);
            Assert.IsFalse(controller.MovementEnabled);

            // Repeated call must be a harmless no-op.
            Assert.DoesNotThrow(() => controller.SetMovementEnabled(false));
            Assert.IsFalse(controller.MovementEnabled);

            controller.SetMovementEnabled(true);
            Assert.IsTrue(controller.MovementEnabled);
        }

        [Test]
        public void ClickToMove_CommandsAreSafe_WithoutNavMesh()
        {
            IPlayerController2D controller = NewClickToMove();

            Assert.DoesNotThrow(() => controller.Stop());
            Assert.DoesNotThrow(() => controller.Pause());
            Assert.DoesNotThrow(() => controller.Resume());
            Assert.DoesNotThrow(() => controller.MoveTo(Vector3.zero));
            Assert.IsFalse(controller.IsMoving);
        }

        [Test]
        public void IAController_ImplementsMovementControl_TogglesEnabled()
        {
            _go = new GameObject("ia");
            IMovementControl controller = _go.AddComponent<IAController2D>();

            Assert.IsTrue(controller.MovementEnabled);

            controller.SetMovementEnabled(false);
            Assert.IsFalse(controller.MovementEnabled);
            Assert.DoesNotThrow(() => controller.Stop());
        }

        // Note: PlayerController2D is intentionally not constructed here. Its
        // [RequireComponent(typeof(IPlayerMovement))] makes AddComponent fail in edit mode
        // (Unity cannot add an interface), which is a harness limitation, not a runtime issue.
        // Its IMovementControl gate is structurally identical to the two controllers above.

        private IPlayerController2D NewClickToMove()
        {
            _go = new GameObject("player");
            return _go.AddComponent<ClickToMoveController2D>();
        }
    }
}
