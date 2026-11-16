using UnityEngine;
using BasketballSim.Core;

namespace BasketballSim.Motion
{
    /// <summary>
    /// Procedural baseline controller. Honest about what it is: it
    /// produces readable locomotion + dribble + shot timing from
    /// local phases, with no neural network. It exists so the game
    /// loop, ball physics, and AI can be tested before any mocap or
    /// trained model exists — and so the neural controller has a
    /// baseline it has to beat.
    /// </summary>
    public sealed class ProceduralMotionController : IMotionController
    {
        private PoseState _state;

        public void Reset(PoseState initial) => _state = initial;

        public PoseDelta Step(PoseState current, GameIntent intent, float dt)
        {
            _state = current;

            // Legs: frequency follows speed; arms: dribble at ~2.2 Hz
            // when moving with the ball, slower when stationary.
            float legFreq = 0.9f + intent.Speed01 * 1.6f;
            _state.LeftLeg.Frequency = legFreq;
            _state.RightLeg.Frequency = legFreq;
            _state.LeftLeg.Advance(dt);
            _state.RightLeg.Position = Mathf.Repeat(_state.LeftLeg.Position + 0.5f, 1f);

            _state.DribbleArm.Frequency = intent.Speed01 > 0.05f ? 2.2f : 1.6f;
            _state.DribbleArm.Advance(dt);

            // Shot: gather while held, release on button release.
            if (intent.WantsShot && _state.ShotPhase < 1f)
                _state.ShotPhase = Mathf.Min(1f, _state.ShotPhase + dt / 0.45f);
            else if (intent.ShotReleased)
                _state.ShotPhase = 1f;

            var delta = new PoseDelta
            {
                RootVelocity = new Vector3(intent.Move.x, 0f, intent.Move.y)
                    * (2.2f + intent.Speed01 * 4.3f),
                DribbleBounce = 0.5f - 0.5f * _state.DribbleArm.Cos,
                ShotHeight01 = _state.ShotPhase,
                DribbleHandTarget = DribbleHandPosition(_state),
                OffHandTarget = _state.RootPosition + Vector3.up * 1.1f,
            };
            return delta;
        }

        private static Vector3 DribbleHandPosition(PoseState s)
        {
            float side = s.DribblingLeft ? -1f : 1f;
            return s.RootPosition
                + s.RootRotation * new Vector3(0.45f * side, 0f, 0.25f)
                + Vector3.up * (0.55f + 0.35f * s.DribbleArm.Sin);
        }
    }
}
