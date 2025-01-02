using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Powerless.Core
{
    public class BattleSystem : MonoBehaviour 
    {
        private Dictionary<Card.CardType, Dictionary<Card.CardType, int>> scoreMatrix;

        private void Awake()
        {
            InitializeScoreMatrix();
        }

        private void InitializeScoreMatrix()
        {
            scoreMatrix = new Dictionary<Card.CardType, Dictionary<Card.CardType, int>>();

            // Initialize Rock outcomes
            scoreMatrix[Card.CardType.Rock] = new Dictionary<Card.CardType, int>
            {
                { Card.CardType.Rock, 1 },     // Draw
                { Card.CardType.Paper, 0 },    // Lose
                { Card.CardType.Scissors, 3 }, // Win
                { Card.CardType.Fire, 0 },     // Lose to Fire
                { Card.CardType.Water, 0 }     // Lose to Water
            };

            // Initialize Paper outcomes
            scoreMatrix[Card.CardType.Paper] = new Dictionary<Card.CardType, int>
            {
                { Card.CardType.Rock, 3 },     // Win
                { Card.CardType.Paper, 1 },    // Draw
                { Card.CardType.Scissors, 0 }, // Lose
                { Card.CardType.Fire, 0 },     // Lose to Fire
                { Card.CardType.Water, 0 }     // Lose to Water
            };

            // Initialize Scissors outcomes
            scoreMatrix[Card.CardType.Scissors] = new Dictionary<Card.CardType, int>
            {
                { Card.CardType.Rock, 0 },     // Lose
                { Card.CardType.Paper, 3 },    // Win
                { Card.CardType.Scissors, 1 }, // Draw
                { Card.CardType.Fire, 0 },     // Lose to Fire
                { Card.CardType.Water, 0 }     // Lose to Water
            };

            // Initialize Fire outcomes
            scoreMatrix[Card.CardType.Fire] = new Dictionary<Card.CardType, int>
            {
                { Card.CardType.Rock, 3 },     // Win
                { Card.CardType.Paper, 3 },    // Win
                { Card.CardType.Scissors, 3 }, // Win
                { Card.CardType.Fire, 1 },     // Draw
                { Card.CardType.Water, 0 }     // Lose to Water
            };

            // Initialize Water outcomes
            scoreMatrix[Card.CardType.Water] = new Dictionary<Card.CardType, int>
            {
                { Card.CardType.Rock, 3 },     // Win
                { Card.CardType.Paper, 3 },    // Win
                { Card.CardType.Scissors, 3 }, // Win
                { Card.CardType.Fire, 3 },     // Win
                { Card.CardType.Water, 1 }     // Draw
            };
        }

        public int CalculateScore(Card playerCard, Card computerCard, bool isReversed = false)
        {
            if (playerCard == null || computerCard == null)
            {
                Debug.LogError("Null card in CalculateScore!");
                return 0;
            }

            Card.CardType playerType = playerCard.GetCardType();
            Card.CardType computerType = computerCard.GetCardType();

            Debug.Log($"Before calculation - Player: {playerType}, Computer: {computerType}");

            // Special handling for Reverse card itself
            if (playerType == Card.CardType.Reverse)
            {
                Debug.Log("Reverse card itself doesn't score points");
                return 0;
            }

            if (!scoreMatrix.ContainsKey(playerType))
            {
                Debug.LogError($"No score matrix for player type: {playerType}");
                return 0;
            }

            if (!scoreMatrix[playerType].ContainsKey(computerType))
            {
                Debug.LogError($"No score defined for: Player {playerType} vs Computer {computerType}");
                return 0;
            }

            int score = scoreMatrix[playerType][computerType];
            
            // If reversed, convert lose to win and win to lose (0 becomes 3, 3 becomes 0)
            // Draw (1) remains the same
            if (isReversed && score != 1)
            {
                score = (score == 0) ? 3 : 0;
                Debug.Log($"Reverse card effect: Score changed to {score}");
            }

            Debug.Log($"Final calculation: {playerType} vs {computerType} = {score} points" + (isReversed ? " (Reversed)" : ""));
            return score;
        }

        public bool IsValidReverseTarget(Card.CardType targetType)
        {
            // Reverse can be used on any card 
            return targetType != Card.CardType.Reverse;
        }
    }
}