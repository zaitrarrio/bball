using UnityEngine;
using BasketballSim.Game;
using BasketballSim.Player;

namespace BasketballSim.Core
{
    /// <summary>
    /// Wires the 1v1 prototype together in the Editor: builds the
    /// court, spawns the two players and the ball, and hands the ball
    /// to the offensive player. Press Play in Main.unity.
    /// </summary>
    public sealed class BasketballBootstrap : MonoBehaviour
    {
        public GameObject PlayerPrefab;
        public GameObject BallPrefab;
        public GameRules1v1 Rules;

        private void Awake()
        {
            CourtBuilder.Build(transform);

            var ball = Instantiate(BallPrefab, new Vector3(0f, 1f, 0f), Quaternion.identity);
            var ballController = ball.GetComponent<Ball.BallController>();

            var player = Instantiate(PlayerPrefab, new Vector3(0f, 0f, 2f), Quaternion.identity);
            var playerController = player.GetComponent<PlayerController>();
            playerController.Ball = ballController;
            playerController.GiveBall();

            var defender = Instantiate(PlayerPrefab, new Vector3(0f, 0f, -2f), Quaternion.identity);
            defender.name = "Defender";
            // Defender uses DefenderAI for intent; its PlayerController
            // is disabled in favour of the AI-driven path in the full
            // build — see docs/DESIGN.md.
        }
    }
}
