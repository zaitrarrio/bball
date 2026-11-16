using UnityEngine;

namespace BasketballSim.Game
{
    /// <summary>
    /// 1v1 rules only — no animation, no input. First to 11, win by 2.
    /// Check-ball after a make, 12-second shot clock, possession
    /// changes on a make, a defensive rebound, or a steal.
    /// </summary>
    public sealed class GameRules1v1 : MonoBehaviour
    {
        public static Vector3 RimTarget = new Vector3(0f, 3.05f, -6.4f);

        public int PlayerScore { get; private set; }
        public int DefenderScore { get; private set; }
        public float ShotClock { get; private set; } = 12f;
        public bool IsOver { get; private set; }

        public int TargetScore = 11;
        public int WinBy = 2;

        public void Score(bool byPlayer, bool isThree)
        {
            int points = isThree ? 3 : 2; // 1v1 half-court: long 2 counts as 2, beyond arc as 3
            if (byPlayer) PlayerScore += points; else DefenderScore += points;
            ShotClock = 12f;
            CheckWin();
        }

        public void OnStealOrRebound() => ShotClock = 12f;

        private void Update()
        {
            if (IsOver) return;
            ShotClock -= Time.deltaTime;
            if (ShotClock <= 0f)
            {
                ShotClock = 12f;
                // Shot-clock violation: possession changes.
            }
        }

        private void CheckWin()
        {
            if ((PlayerScore >= TargetScore || DefenderScore >= TargetScore)
                && Mathf.Abs(PlayerScore - DefenderScore) >= WinBy)
                IsOver = true;
        }

        public string ScoreLine() => $"{PlayerScore} — {DefenderScore}";
    }
}
