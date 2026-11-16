using UnityEngine;

namespace BasketballSim.Core
{
    /// <summary>
    /// The only thing the game brain / human input produces.
    /// Human players and AI both output a GameIntent, so they are
    /// interchangeable. The motion layer never reads raw input.
    /// </summary>
    public struct GameIntent
    {
        public Vector2 Move;          // x = strafe, y = forward, -1..1
        public float Speed01;         // 0 = idle, 1 = sprint
        public bool WantsShot;        // shoot button held
        public bool ShotReleased;     // shoot button released this frame
        public bool WantsCrossover;   // crossover pressed this frame
        public bool WantsSteal;       // defender reach pressed this frame
        public Vector3 FacingTarget;  // where the player wants to face
    }

    public enum Possession { Player, Defender, Loose, Dead }
}
