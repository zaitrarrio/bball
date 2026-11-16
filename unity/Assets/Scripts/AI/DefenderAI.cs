using UnityEngine;
using BasketballSim.Core;

namespace BasketballSim.AI
{
    /// <summary>
    /// 1v1 defender. Decisions only — it outputs a GameIntent, exactly
    /// like a human. Rules: stay between ball and basket, close out
    /// when the shot gathers, reach only on a predictable dribble
    /// rhythm (and get punished on a crossover, because a crossover
    /// breaks that rhythm).
    /// </summary>
    public sealed class DefenderAI : MonoBehaviour
    {
        public Transform BallHandler;
        public Transform Basket;
        public float CloseoutDistance = 1.1f;
        public float ContestDistance = 1.6f;
        public float StealCooldown = 1.2f;

        private float _stealTimer;
        private float _dribbleRhythm;

        public GameIntent Decide(PoseState defender, PoseState handler, bool handlerShooting, float dt)
        {
            _stealTimer -= dt;

            Vector3 toBasket = (Basket.position - transform.position).normalized;
            Vector3 toHandler = BallHandler.position - transform.position;
            float dist = toHandler.magnitude;

            // Positioning: shade between handler and basket.
            Vector3 guardSpot = BallHandler.position + toBasket * -0.9f;
            Vector3 toSpot = guardSpot - transform.position;
            var intent = new GameIntent
            {
                Move = new Vector2(toSpot.x, toSpot.z).normalized,
                Speed01 = Mathf.Clamp01(toSpot.magnitude),
                FacingTarget = BallHandler.position,
            };

            if (handlerShooting && dist < ContestDistance)
            {
                // Contest: close the last step and raise a hand.
                intent.Move = new Vector2(toHandler.x, toHandler.z).normalized;
                intent.Speed01 = 1f;
                intent.WantsShot = true; // reused as "contest / jump" here
            }
            else if (dist < CloseoutDistance && _stealTimer <= 0f)
            {
                // Reach only when the dribble is predictable — i.e.
                // the handler has not crossed over recently. The
                // caller tracks that; a wrong guess leaves the
                // defender out of position, which is the point.
                _dribbleRhythm += dt;
                if (_dribbleRhythm > 1.4f)
                {
                    intent.WantsSteal = true;
                    _stealTimer = StealCooldown;
                    _dribbleRhythm = 0f;
                }
            }

            return intent;
        }

        public void OnCrossover() => _dribbleRhythm = 0f; // rhythm broken
    }
}
