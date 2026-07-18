using NUnit.Framework;

namespace MyUnityPackage.Controller.EditorTests
{
    /// <summary>
    /// Edit-mode tests for the pure waypoint stepper <see cref="IAController2D.NextIndex"/>
    /// (exposed via InternalsVisibleTo). Covers the single-point and out-of-range cases that
    /// previously threw <see cref="System.IndexOutOfRangeException"/>.
    /// </summary>
    public class IAControllerPathTests
    {
        [Test]
        public void SinglePoint_AlwaysReturnsZero()
        {
            int dir = 1;
            Assert.AreEqual(0, IAController2D.NextIndex(-1, 1, IAController2D.PathMode.Loop, ref dir));
            Assert.AreEqual(0, IAController2D.NextIndex(0, 1, IAController2D.PathMode.Loop, ref dir));
            Assert.AreEqual(0, IAController2D.NextIndex(-1, 1, IAController2D.PathMode.PingPong, ref dir));
            Assert.AreEqual(0, IAController2D.NextIndex(0, 1, IAController2D.PathMode.PingPong, ref dir));
        }

        [Test]
        public void EmptyPath_ReturnsMinusOne()
        {
            int dir = 1;
            Assert.AreEqual(-1, IAController2D.NextIndex(-1, 0, IAController2D.PathMode.Loop, ref dir));
            Assert.AreEqual(-1, IAController2D.NextIndex(0, 0, IAController2D.PathMode.PingPong, ref dir));
        }

        [Test]
        public void Loop_WrapsAround()
        {
            int dir = 1;
            int index = -1;
            int[] expected = { 0, 1, 2, 0, 1 };
            foreach (int e in expected)
            {
                index = IAController2D.NextIndex(index, 3, IAController2D.PathMode.Loop, ref dir);
                Assert.AreEqual(e, index);
            }
        }

        [Test]
        public void PingPong_BouncesAtEnds()
        {
            int dir = 1;
            int index = -1;
            int[] expected = { 0, 1, 2, 1, 0, 1 };
            foreach (int e in expected)
            {
                index = IAController2D.NextIndex(index, 3, IAController2D.PathMode.PingPong, ref dir);
                Assert.AreEqual(e, index);
            }
        }

        [Test]
        public void PingPong_TwoPoints_Alternates()
        {
            int dir = 1;
            int index = -1;
            int[] expected = { 0, 1, 0, 1, 0 };
            foreach (int e in expected)
            {
                index = IAController2D.NextIndex(index, 2, IAController2D.PathMode.PingPong, ref dir);
                Assert.AreEqual(e, index);
            }
        }

        [Test]
        public void OutOfRangeIndex_IsClamped()
        {
            int dir = 1;
            Assert.AreEqual(0, IAController2D.NextIndex(10, 3, IAController2D.PathMode.Loop, ref dir));

            dir = 1;
            Assert.AreEqual(1, IAController2D.NextIndex(10, 3, IAController2D.PathMode.PingPong, ref dir));
        }
    }
}
