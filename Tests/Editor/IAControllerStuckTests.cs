using NUnit.Framework;

namespace MyUnityPackage.Controller.EditorTests
{
    /// <summary>
    /// Edit-mode tests for the pure anti-stuck decision logic of <see cref="IAController2D"/>
    /// (exposed via InternalsVisibleTo): <see cref="IAController2D.EvaluateStuck"/> and
    /// <see cref="IAController2D.BlockedArrivalTolerance"/>.
    /// </summary>
    public class IAControllerStuckTests
    {
        [Test]
        public void BelowTimeout_ReturnsNotStuck()
        {
            Assert.AreEqual(
                IAController2D.StuckResolution.NotStuck,
                IAController2D.EvaluateStuck(1.0f, 1.5f, 0.5f, 0.6f));
        }

        [Test]
        public void ZeroOrNegativeTimeout_DisablesWatchdog()
        {
            Assert.AreEqual(
                IAController2D.StuckResolution.NotStuck,
                IAController2D.EvaluateStuck(999f, 0f, 0.5f, 0.6f));
            Assert.AreEqual(
                IAController2D.StuckResolution.NotStuck,
                IAController2D.EvaluateStuck(999f, -1f, 0.5f, 0.6f));
        }

        [Test]
        public void AtTimeout_WithinTolerance_TreatsAsArrived()
        {
            Assert.AreEqual(
                IAController2D.StuckResolution.TreatAsArrived,
                IAController2D.EvaluateStuck(1.5f, 1.5f, 0.5f, 0.6f));

            // Boundary: remaining exactly at tolerance still counts as arrived.
            Assert.AreEqual(
                IAController2D.StuckResolution.TreatAsArrived,
                IAController2D.EvaluateStuck(1.5f, 1.5f, 0.6f, 0.6f));
        }

        [Test]
        public void AtTimeout_BeyondTolerance_Repaths()
        {
            Assert.AreEqual(
                IAController2D.StuckResolution.Repath,
                IAController2D.EvaluateStuck(1.5f, 1.5f, 3f, 0.6f));
        }

        [Test]
        public void InfiniteRemainingDistance_Repaths()
        {
            // remainingDistance is +Infinity on an invalid/partial path.
            Assert.AreEqual(
                IAController2D.StuckResolution.Repath,
                IAController2D.EvaluateStuck(2f, 1.5f, float.PositiveInfinity, 0.6f));
        }

        [Test]
        public void BlockedArrivalTolerance_UsesLargerOfStoppingAndThreshold_PlusTwoRadii()
        {
            Assert.AreEqual(0.6f, IAController2D.BlockedArrivalTolerance(0.2f, 0.15f, 0.2f), 1e-5f);
            Assert.AreEqual(1.21f, IAController2D.BlockedArrivalTolerance(0f, 0.15f, 0.53f), 1e-5f);
        }
    }
}
