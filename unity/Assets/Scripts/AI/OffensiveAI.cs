using UnityEngine;
using BasketballSim.Core;

namespace BasketballSim.AI
{
    /// <summary>
    /// Simple offensive AI for AI-vs-AI demo mode. Same GameIntent
    /// output as a human: drive when open, shoot when the defender
    /// sags, cross over when the defender over-commits.
    /// </summary>
    public sealed class OffensiveAI : MonoBehaviour
    {
        public Transform Defender;
        public Transform Basket;

        public GameIntent Decide(Vector3 selfPos, float shotClock, float dt)
        {
            float distToDefender = Vector3.Distance(selfPos, Defender.position);
            float distToBasket = Vector3.Distance(selfPos, Basket.position);
            var intent = new GameIntent { FacingTarget = Basket.position };

            bool defenderSagging = distToDefender > 2.2f;
            bool inRange = distToBasket < 7.5f;

            if (defenderSagging && inRange)
            {
                intent.WantsShot = true;
            }
            else if (shotClock < 4f && inRange)
            {
                intent.WantsShot = true;
            }
            else
            {
                // Drive toward the basket, cross over if the defender
                // is square in front of us.
                Vector3 toBasket = (Basket.position - selfPos).normalized;
                intent.Move = new Vector2(toBasket.x, toBasket.z);
                intent.Speed01 = 0.8f;
                if (distToDefender < 1.0f)
                    intent.WantsCrossover = true;
            }

            return intent;
        }
    }
}
