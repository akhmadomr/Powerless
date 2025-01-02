using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Powerless.UI;

namespace Powerless.Core
{
    public class DeckManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] cardPrefabs;
        private List<Card> playerDeck = new List<Card>();
        private CardButton[] cardButtons;

        void Awake()
        {
            if (cardPrefabs == null || cardPrefabs.Length == 0)
            {
                Debug.LogError("Card prefabs not assigned in DeckManager!");
                return;
            }
        }
        
        void Start()
        {
            cardButtons = FindObjectsOfType<CardButton>();
            if (cardButtons == null || cardButtons.Length == 0)
            {
                Debug.LogError("No CardButtons found in scene!");
                return;
            }
            
            // Don't generate initial deck here - let GameManager call it
        }

        public void GenerateInitialDeck()
        {
            Debug.Log("Generating initial deck...");
            
            if (cardPrefabs == null || cardPrefabs.Length == 0)
            {
                Debug.LogError("Card prefabs array is empty!");
                return;
            }

            if (cardButtons == null || cardButtons.Length == 0)
            {
                cardButtons = FindObjectsOfType<CardButton>();
                if (cardButtons == null || cardButtons.Length == 0)
                {
                    Debug.LogError("No CardButtons found!");
                    return;
                }
            }

            // Generate cards
            for(int i = 0; i < Mathf.Min(10, cardButtons.Length); i++)
            {
                try
                {
                    int randomIndex = Random.Range(0, 3); // Rock, Paper, Scissors only
                    if (cardPrefabs[randomIndex] == null)
                    {
                        Debug.LogError($"Card prefab at index {randomIndex} is null!");
                        continue;
                    }

                    GameObject cardObj = Instantiate(cardPrefabs[randomIndex], transform);
                    Card card = cardObj.GetComponent<Card>();
                    Image cardImage = cardObj.GetComponent<Image>();

                    if (card != null && cardImage != null && cardButtons[i] != null)
                    {
                        playerDeck.Add(card);
                        CardButton cardButton = cardButtons[i];
                        cardButton.SetCard(card);
                        Debug.Log($"Card {i} generated: {card.GetCardType()}");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error generating card {i}: {e.Message}");
                }
            }
        }
        
        public Card GetRandomSpecialCard()
        {
            int randomIndex = Random.Range(3, 6); // Fire (3), Water (4), Reverse (5)
            Debug.Log($"Creating special card with index: {randomIndex}");
            
            GameObject cardObj = Instantiate(cardPrefabs[randomIndex], transform);
            Card card = cardObj.GetComponent<Card>();
            
            if (card != null)
            {
                // Make sure the card is fully visible
                Image cardImage = cardObj.GetComponent<Image>();
                if (cardImage != null)
                {
                    Color color = cardImage.color;
                    color.a = 1f;
                    cardImage.color = color;
                }
                Debug.Log($"Created special card of type: {card.GetCardType()}");
            }
            
            return card;
        }

        public GameObject GetCardPrefab(int index)
        {
            // Never allow Reverse card (index 5) for computer
            if (index >= 0 && index < cardPrefabs.Length && index != 5)
            {
                Card card = cardPrefabs[index].GetComponent<Card>();
                if (card != null && card.GetCardType() != Card.CardType.Reverse)
                {
                    return cardPrefabs[index];
                }
            }
            
            Debug.LogWarning($"Attempted to get invalid card type with index: {index}");
            return null;
        }
        
        public List<Card> GetPlayerDeck()
        {
            return playerDeck;
        }
    }

}