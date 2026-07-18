using UnityEngine;

namespace MyUnityPackage.Controller
{
    /// <summary>
    /// Base class that drives a 2D <see cref="UnityEngine.Animator"/> from a velocity source.
    /// It writes a (optionally snapped) facing direction to the MoveX/MoveY parameters and the
    /// raw speed to the Speed parameter. Concrete subclasses only provide the velocity source
    /// (Rigidbody2D, NavMeshAgent, ...). Animator parameter names are hashed in <see cref="Awake"/>.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public abstract class DirectionalAnimator2D : MonoBehaviour
    {
        [Header("Animator parameter names")]
        [SerializeField] private string moveXParam = "MoveX";
        [SerializeField] private string moveYParam = "MoveY";
        [SerializeField] private string speedParam = "Speed";

        [Header("Direction")]
        [Tooltip("Quantizes the facing written to MoveX/MoveY. Free = no snapping, Snap4 = cardinals, Snap8 = cardinals + diagonals. The Speed parameter stays continuous.")]
        [SerializeField] private DirectionMode directionMode = DirectionMode.Free;

        /// <summary>Speed above which the facing direction is refreshed from the velocity.</summary>
        protected const float SpeedEpsilon = 0.01f;

        private Animator animator;
        private int moveXHash;
        private int moveYHash;
        private int speedHash;

        private Vector2 lastMoveDirection = Vector2.down; // raw normalized facing (default: down)
        private Vector2 facingDirection = Vector2.down;   // last quantized value written

        /// <summary>The Animator being driven (resolved in <see cref="Awake"/>).</summary>
        protected Animator Animator => animator;

        /// <summary>Runtime-switchable quantization mode for the facing direction.</summary>
        public DirectionMode DirectionMode
        {
            get => directionMode;
            set => directionMode = value;
        }

        /// <summary>The last facing direction written to the animator (quantized per <see cref="DirectionMode"/>).</summary>
        public Vector2 FacingDirection => facingDirection;

        /// <summary>Resolves the Animator and caches parameter hashes. Override to add subclass setup (call base first).</summary>
        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
            moveXHash = UnityEngine.Animator.StringToHash(moveXParam);
            moveYHash = UnityEngine.Animator.StringToHash(moveYParam);
            speedHash = UnityEngine.Animator.StringToHash(speedParam);
        }

        /// <summary>Returns the current velocity used to drive the animation (subclass-specific source).</summary>
        protected abstract Vector2 ReadVelocity();

        /// <summary>Hook to write extra animator parameters (e.g. bools). Called at the end of <see cref="Update"/>.</summary>
        protected virtual void ApplyExtraParameters() { }

        /// <summary>Reads the velocity, updates facing/speed parameters, then applies extra parameters.</summary>
        protected virtual void Update()
        {
            Vector2 velocity = ReadVelocity();
            float speed = velocity.magnitude;

            if (speed > SpeedEpsilon)
                lastMoveDirection = velocity / speed;

            facingDirection = DirectionQuantizer.Quantize(lastMoveDirection, directionMode);

            animator.SetFloat(moveXHash, facingDirection.x);
            animator.SetFloat(moveYHash, facingDirection.y);
            animator.SetFloat(speedHash, speed); // raw magnitude — never snapped

            ApplyExtraParameters();
        }
    }
}
