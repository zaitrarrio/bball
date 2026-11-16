using UnityEngine;

namespace BasketballSim.Ball
{
    public enum BallState { Dribbled, Held, InFlight, Loose, Dead }

    /// <summary>
    /// The ball is a separate simulated rigid body — never baked into
    /// an animation clip, never parented to a hand. While dribbling
    /// the hand is driven TO the ball by IK; while held the ball is
    /// constrained to the shooting pocket. Shots are solved as a real
    /// projectile to a target entry point over the rim.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class BallController : MonoBehaviour
    {
        public BallState State { get; private set; } = BallState.Dead;
        public float Radius = 0.12f;          // ~size 7 ball, metres
        public float BounceRestitution = 0.82f;

        private Rigidbody _rb;
        private Transform _holder;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.useGravity = true;
            _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        public void StartDribble(Transform holder)
        {
            _holder = holder;
            State = BallState.Dribbled;
            _rb.isKinematic = true;
        }

        public void Hold(Transform holder)
        {
            _holder = holder;
            State = BallState.Held;
            _rb.isKinematic = true;
        }

        /// <summary>Solve a launch velocity that passes through <paramref name="target"/>
        /// after <paramref name="flightTime"/> seconds, under gravity.</summary>
        public void Shoot(Vector3 target, float flightTime, float makeProbability)
        {
            State = BallState.InFlight;
            _rb.isKinematic = false;
            // Misses are applied as a lateral offset at the rim, so a
            // miss still looks like a real shot, not a random fling.
            float miss = Random.value > makeProbability ? Random.Range(0.09f, 0.22f) : 0f;
            Vector3 aim = target + new Vector3(miss * (Random.value > 0.5f ? 1f : -1f), 0f, 0f);
            Vector3 displacement = aim - transform.position;
            Vector3 velocity = displacement / flightTime;
            velocity.y += 0.5f * Mathf.Abs(Physics.gravity.y) * flightTime;
            _rb.velocity = velocity;
            _rb.angularVelocity = new Vector3(-12f, 0f, 0f); // backspin
        }

        public void GoLoose()
        {
            State = BallState.Loose;
            _holder = null;
            _rb.isKinematic = false;
        }

        private void FixedUpdate()
        {
            if (State == BallState.Dribbled && _holder != null)
            {
                // Dribble bounce is driven by the motion phase in the
                // player controller; here we just follow the hand
                // height target it hands us via the holder transform.
                transform.position = _holder.position;
            }
            else if (State == BallState.Held && _holder != null)
            {
                transform.position = _holder.position;
                transform.rotation = _holder.rotation;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (State != BallState.InFlight && State != BallState.Loose) return;
            // Floor / rim / backboard bounce is handled by the physics
            // material; this hook is where swish vs. rim-out is scored
            // by the game rules (see GameRules1v1).
        }
    }
}
