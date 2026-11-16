using UnityEngine;

namespace BasketballSim.Motion
{
    /// <summary>
    /// One local phase per limb group — the core idea from AI4Animation
    /// (Starke et al., SIGGRAPH 2020). A phase is a 0..1 cycle position
    /// plus a frequency. Legs and arms run independently, so a player
    /// can dribble (arm phase) while changing direction (leg phase)
    /// without snapping to a shared clip boundary.
    /// </summary>
    public struct LocalPhase
    {
        public float Position;   // 0..1 around the cycle
        public float Frequency;  // cycles per second

        public void Advance(float dt) =>
            Position = Mathf.Repeat(Position + Frequency * dt, 1f);

        public float Sin => Mathf.Sin(Position * Mathf.PI * 2f);
        public float Cos => Mathf.Cos(Position * Mathf.PI * 2f);
    }

    public struct PoseState
    {
        public Vector3 RootPosition;
        public Quaternion RootRotation;
        public Vector3 RootVelocity;
        public LocalPhase LeftLeg;
        public LocalPhase RightLeg;
        public LocalPhase DribbleArm;   // arm currently dribbling
        public LocalPhase OffArm;
        public LocalPhase Torso;
        public float ShotPhase;         // 0 = no shot, 0..1 gather->release
        public bool DribblingLeft;
    }

    public struct PoseDelta
    {
        public Vector3 RootVelocity;
        public float DribbleBounce;     // 0..1, 1 = ball at hand height
        public float ShotHeight01;      // 0 = pocket, 1 = release
        public Vector3 DribbleHandTarget;
        public Vector3 OffHandTarget;
    }
}
