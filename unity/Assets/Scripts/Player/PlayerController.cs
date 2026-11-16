using UnityEngine;
using BasketballSim.Ball;
using BasketballSim.Core;
using BasketballSim.Motion;

namespace BasketballSim.Player
{
    /// <summary>
    /// Human 1v1 player. Reads input into a GameIntent, hands it to
    /// the motion controller, moves the root, and owns the ball while
    /// on offence. The defender is the same shape of object with an
    /// AI intent source instead of input — see AI/DefenderAI.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        public BallController Ball;
        public Transform DribbleHandTarget;
        public Transform ShootingPocket;
        public float CrossoverCooldown = 0.6f;

        private IMotionController _motion = new ProceduralMotionController();
        private PoseState _pose;
        private CharacterController _cc;
        private float _crossoverTimer;
        private bool _hasBall = true;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _pose = new PoseState
            {
                RootPosition = transform.position,
                RootRotation = transform.rotation,
                DribblingLeft = false,
            };
            _motion.Reset(_pose);
        }

        private void Update()
        {
            _crossoverTimer -= Time.deltaTime;

            var intent = new GameIntent
            {
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
                Speed01 = Input.GetKey(KeyCode.LeftShift) ? 1f : 0.6f,
                WantsShot = Input.GetKey(KeyCode.Space),
                ShotReleased = Input.GetKeyUp(KeyCode.Space),
                WantsCrossover = Input.GetKeyDown(KeyCode.LeftControl) && _crossoverTimer <= 0f,
            };

            if (intent.WantsCrossover && _hasBall)
            {
                _pose.DribblingLeft = !_pose.DribblingLeft;
                _crossoverTimer = CrossoverCooldown;
                // Burst the other way: the crossover is a move, not a clip.
                intent.Move.x = _pose.DribblingLeft ? -1f : 1f;
            }

            _pose.RootPosition = transform.position;
            _pose.RootRotation = transform.rotation;
            PoseDelta delta = _motion.Step(_pose, intent, Time.deltaTime);

            _cc.Move(delta.RootVelocity * Time.deltaTime);
            if (delta.RootVelocity.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(delta.RootVelocity), Time.deltaTime * 10f);

            if (intent.ShotReleased && _hasBall)
            {
                // Shot quality is decided by the game rules (openness,
                // timing); here we just launch the ball.
                Ball.Shoot(Game.GameRules1v1.RimTarget, 0.85f, 0.55f);
                _hasBall = false;
                _pose.ShotPhase = 0f;
            }

            if (DribbleHandTarget != null)
                DribbleHandTarget.position = delta.DribbleHandTarget;
        }

        public void GiveBall()
        {
            _hasBall = true;
            Ball.Hold(ShootingPocket);
        }
    }
}
