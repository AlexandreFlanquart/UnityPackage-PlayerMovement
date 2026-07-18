using UnityEngine;

namespace MyUnityPackage.Controller
{
    /// <summary>
    /// How a facing direction is quantized before being written to the MoveX/MoveY animator
    /// parameters. Only the direction is snapped; the Speed parameter always stays continuous.
    /// </summary>
    public enum DirectionMode
    {
        /// <summary>No snapping: the raw normalized direction is used.</summary>
        Free = 0,

        /// <summary>Snap to the 4 cardinal directions (up/down/left/right).</summary>
        Snap4 = 1,

        /// <summary>Snap to the 4 cardinals plus the 4 normalized diagonals (8 directions).</summary>
        Snap8 = 2
    }

    /// <summary>
    /// Pure, allocation-free helper that snaps a 2D facing direction to a fixed set of
    /// directions. Outputs are exact unit vectors, which lets FreeformDirectional2D blend
    /// trees select the intended child cleanly (cardinals at (±1,0)/(0,±1), diagonals at
    /// (±0.70710678, ±0.70710678)). Reusable outside the animators.
    /// </summary>
    public static class DirectionQuantizer
    {
        // Unit length of a 45° diagonal component (1/sqrt(2)).
        private const float Diagonal = 0.70710678f;

        // East, North, West, South (counter-clockwise from +X).
        private static readonly Vector2[] Cardinal4 =
        {
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(-1f, 0f),
            new Vector2(0f, -1f)
        };

        // East, NE, North, NW, West, SW, South, SE (counter-clockwise from +X).
        private static readonly Vector2[] Direction8 =
        {
            new Vector2(1f, 0f),
            new Vector2(Diagonal, Diagonal),
            new Vector2(0f, 1f),
            new Vector2(-Diagonal, Diagonal),
            new Vector2(-1f, 0f),
            new Vector2(-Diagonal, -Diagonal),
            new Vector2(0f, -1f),
            new Vector2(Diagonal, -Diagonal)
        };

        /// <summary>
        /// Snaps <paramref name="direction"/> according to <paramref name="mode"/>.
        /// A near-zero input returns <see cref="Vector2.zero"/>. The input magnitude is
        /// otherwise ignored (only its angle matters); boundary angles snap to the
        /// counter-clockwise sector.
        /// </summary>
        /// <param name="direction">Facing direction (does not need to be normalized).</param>
        /// <param name="mode">Quantization mode.</param>
        /// <returns>An exact unit vector, or zero for a near-zero input.</returns>
        public static Vector2 Quantize(Vector2 direction, DirectionMode mode)
        {
            if (direction.sqrMagnitude < 1e-6f)
                return Vector2.zero;

            switch (mode)
            {
                case DirectionMode.Snap4:
                    return Cardinal4[SectorIndex(direction, 4)];
                case DirectionMode.Snap8:
                    return Direction8[SectorIndex(direction, 8)];
                default:
                    return direction.normalized;
            }
        }

        // Index of the sector [0, sectorCount) whose center is closest to the direction's angle.
        private static int SectorIndex(Vector2 direction, int sectorCount)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // [-180, 180]
            float sectorSize = 360f / sectorCount;

            // Round half-up so boundary angles fall into the counter-clockwise sector.
            int sector = Mathf.FloorToInt(angle / sectorSize + 0.5f);
            return ((sector % sectorCount) + sectorCount) % sectorCount;
        }
    }
}
