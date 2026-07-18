using NUnit.Framework;
using UnityEngine;

namespace MyUnityPackage.Controller.EditorTests
{
    /// <summary>
    /// Edit-mode tests for the pure <see cref="DirectionQuantizer"/> helper (no scene needed).
    /// </summary>
    public class DirectionQuantizerTests
    {
        private const float Tolerance = 1e-4f;

        private static Vector2 Dir(float degrees)
        {
            float rad = degrees * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        }

        private static void AssertApprox(Vector2 expected, Vector2 actual)
        {
            Assert.AreEqual(expected.x, actual.x, Tolerance, $"x mismatch (expected {expected}, got {actual})");
            Assert.AreEqual(expected.y, actual.y, Tolerance, $"y mismatch (expected {expected}, got {actual})");
        }

        [Test]
        public void Free_ReturnsNormalized()
        {
            AssertApprox(new Vector2(0.6f, 0.8f), DirectionQuantizer.Quantize(new Vector2(3f, 4f), DirectionMode.Free));
        }

        [TestCase(DirectionMode.Free)]
        [TestCase(DirectionMode.Snap4)]
        [TestCase(DirectionMode.Snap8)]
        public void ZeroVector_ReturnsZero(DirectionMode mode)
        {
            AssertApprox(Vector2.zero, DirectionQuantizer.Quantize(Vector2.zero, mode));
        }

        [Test]
        public void Snap4_SectorCenters()
        {
            AssertApprox(new Vector2(1f, 0f), DirectionQuantizer.Quantize(new Vector2(0.9f, 0.3f), DirectionMode.Snap4));
            AssertApprox(new Vector2(0f, 1f), DirectionQuantizer.Quantize(new Vector2(0.3f, 0.9f), DirectionMode.Snap4));
            AssertApprox(new Vector2(-1f, 0f), DirectionQuantizer.Quantize(new Vector2(-0.9f, -0.3f), DirectionMode.Snap4));
            AssertApprox(new Vector2(0f, -1f), DirectionQuantizer.Quantize(new Vector2(0.3f, -0.9f), DirectionMode.Snap4));
        }

        [Test]
        public void Snap4_NearBoundaries()
        {
            AssertApprox(new Vector2(1f, 0f), DirectionQuantizer.Quantize(Dir(44f), DirectionMode.Snap4));
            AssertApprox(new Vector2(0f, 1f), DirectionQuantizer.Quantize(Dir(46f), DirectionMode.Snap4));
            AssertApprox(new Vector2(1f, 0f), DirectionQuantizer.Quantize(Dir(-44f), DirectionMode.Snap4));
            AssertApprox(new Vector2(-1f, 0f), DirectionQuantizer.Quantize(Dir(136f), DirectionMode.Snap4));
        }

        [Test]
        public void Snap8_SectorCenters()
        {
            const float d = 0.70710678f;
            AssertApprox(new Vector2(1f, 0f), DirectionQuantizer.Quantize(Dir(0f), DirectionMode.Snap8));
            AssertApprox(new Vector2(d, d), DirectionQuantizer.Quantize(Dir(45f), DirectionMode.Snap8));
            AssertApprox(new Vector2(0f, 1f), DirectionQuantizer.Quantize(Dir(90f), DirectionMode.Snap8));
            AssertApprox(new Vector2(-d, d), DirectionQuantizer.Quantize(Dir(135f), DirectionMode.Snap8));
            AssertApprox(new Vector2(-1f, 0f), DirectionQuantizer.Quantize(Dir(180f), DirectionMode.Snap8));
            AssertApprox(new Vector2(-d, -d), DirectionQuantizer.Quantize(Dir(225f), DirectionMode.Snap8));
            AssertApprox(new Vector2(0f, -1f), DirectionQuantizer.Quantize(Dir(270f), DirectionMode.Snap8));
            AssertApprox(new Vector2(d, -d), DirectionQuantizer.Quantize(Dir(315f), DirectionMode.Snap8));
        }

        [Test]
        public void Snap8_NearBoundaries()
        {
            const float d = 0.70710678f;
            AssertApprox(new Vector2(1f, 0f), DirectionQuantizer.Quantize(Dir(21.5f), DirectionMode.Snap8));
            AssertApprox(new Vector2(d, d), DirectionQuantizer.Quantize(Dir(23.5f), DirectionMode.Snap8));
            AssertApprox(new Vector2(1f, 0f), DirectionQuantizer.Quantize(Dir(-21.5f), DirectionMode.Snap8));
            AssertApprox(new Vector2(d, -d), DirectionQuantizer.Quantize(Dir(-23.5f), DirectionMode.Snap8));
        }

        [Test]
        public void Snap8_OutputsAreUnitLength()
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 result = DirectionQuantizer.Quantize(Dir(i * 45f), DirectionMode.Snap8);
                Assert.AreEqual(1f, result.magnitude, Tolerance, $"sector {i} not unit length");
            }
        }
    }
}
