using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Powerless.Core;
using Powerless.UI;

namespace Powerless.Core
{
public class GameManager : MonoBehaviour
{
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Image[] computerBattleSlots = new Image[3];
    
    [SerializeField, Header("UI References")] 
    private CardSelectionManager cardSelectionManager;
    
    // Get the empty slot sprite from CardSelectionManager instead
    private Sprite defaultBattleSlotSprite => cardSelectionManager?.emptySlotSprite;

    private int currentRound = 1;
    private int totalScore = 0;
    private Dictionary<Card.CardType, int> cardCounts = new Dictionary<Card.CardType, int>();
    private Card[] computerCards = new Card[3]; // Add this field to track computer cards
    
    void Awake()
    {
        // Auto-find components on same GameObject
        if (deckManager == null)
        {
            deckManager = GetComponent<DeckManager>();
            if (deckManager == null)
            {
                deckManager = FindObjectOfType<DeckManager>();
            }
            Debug.Log($"DeckManager reference in Awake: {deckManager != null}");
        }

        if (cardSelectionManager == null)
        {
            cardSelectionManager = FindObjectOfType<CardSelectionManager>();
        }
        
        // Initialize computer battle slots if not set
        if (computerBattleSlots == null || computerBattleSlots.Length != 3)
        {
            computerBattleSlots = new Image[3];
            Debug.Log("Initializing computer battle slots array");
        }

        // Try to find slots by tag if not assigned
        bool foundSlots = false;
        try
        {
            GameObject[] slotObjects = GameObject.FindGameObjectsWithTag("ComputerBattleSlot");
            if (slotObjects != null && slotObjects.Length > 0)
            {
                for (int i = 0; i < Mathf.Min(slotObjects.Length, 3); i++)
                {
                    computerBattleSlots[i] = slotObjects[i].GetComponent<Image>();
                    foundSlots = computerBattleSlots[i] != null;
                }
            }
        }
        catch (UnityException)
        {
            Debug.LogWarning("ComputerBattleSlot tag not found. Please assign slots manually in inspector.");
        }

        if (!foundSlots)
        {
            Debug.LogWarning("No computer battle slots found. Make sure they are assigned in inspector or tagged properly.");
        }

        // Initialize computer slots with default sprite immediately
        if (cardSelectionManager != null && cardSelectionManager.emptySlotSprite != null)
        {
            foreach (Image slot in computerBattleSlots)
            {
                if (slot != null)
                {
                    slot.sprite = cardSelectionManager.emptySlotSprite;
                }
            }
        }
    }
     void Start()
    {
        // Find or get DeckManager first
        if (deckManager == null)
        {
            deckManager = GetComponent<DeckManager>();
            if (deckManager == null)
            {
                deckManager = FindObjectOfType<DeckManager>();
                if (deckManager == null)
                {
                    Debug.LogError("DeckManager not found! Please add DeckManager component.");
                    return;
                }
            }
        }

        // Validate computer battle slots
        bool slotsValid = true;
        for (int i = 0; i < 3; i++)
        {
            if (computerBattleSlots[i] == null)
            {
                Debug.LogError($"Computer battle slot {i} is not assigned!");
                slotsValid = false;
            }
        }

        if (!slotsValid)
        {
            Debug.LogError("Computer battle slots not properly set up!");
            return;
        }

        // Initialize computer slots with the same sprite as player slots
        if (cardSelectionManager != null && cardSelectionManager.emptySlotSprite != null)
        {
            foreach (Image slot in computerBattleSlots)
            {
                if (slot != null)
                {
                    slot.sprite = cardSelectionManager.emptySlotSprite;
                }
            }
        }

        // Only proceed if all components are valid
        InitializeGame();
    }
    
    private void InitializeGame()
    {
        if (deckManager == null || uiManager == null)
        {
            Debug.LogError("Required components missing! Check DeckManager and UIManager references.");
            return;
        }

        try
        {
            InitializeCardCounts();
            deckManager.GenerateInitialDeck();
            uiManager.UpdateCardCountUI(cardCounts);
            StartCoroutine(RoundLoop());
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in InitializeGame: {e.Message}");
        }
    }
        private void InitializeCardCounts()
        {
            foreach(Card.CardType type in System.Enum.GetValues(typeof(Card.CardType)))
            {
                cardCounts[type] = 0;
            }
        }
    
    private IEnumerator PlayRound()
    {
        yield return new WaitForSeconds(0.5f); // Small delay for visual feedback
        
        // Rest of battle logic remains unchanged
        // ...existing code...
    }
        private void CalculateRoundScore()
    {
        int roundScore = 0;
        
        for (int i = 0; i < 3; i++)
        {
            Card playerCard = cardSelectionManager.GetCardInSlot(i);
            Card computerCard = computerBattleSlots[i].GetComponentInChildren<Card>();
            
            if (playerCard != null && computerCard != null)
            {
                // Check if this slot has a reverse card effect
                bool isReversed = cardSelectionManager.HasReverseCard(i);
                
                int slotScore = battleSystem.CalculateScore(playerCard, computerCard, isReversed);
                roundScore += slotScore;
                
                string result = slotScore == 3 ? "WIN" : (slotScore == 1 ? "DRAW" : "LOSE");
                string reversedText = isReversed ? " (Reversed)" : "";
                Debug.Log($"Slot {i}: {playerCard.GetCardType()} vs {computerCard.GetCardType()} = {result} ({slotScore} points){reversedText}");
            }
        }

        totalScore += roundScore;
        uiManager.UpdateScore(totalScore);
        Debug.Log($"Round Score: {roundScore}, Total Score: {totalScore}");
    }
        private void EndGame()
        {
            Debug.Log($"Game Over! Total Score: {totalScore}");
            uiManager.UpdateScore(totalScore);
            // Add game over UI logic here
        }
        private IEnumerator RoundLoop()
{
    while (currentRound <= 3) // 3 rounds total
    {
        Debug.Log($"Starting Round {currentRound}");
        
        // Reset round state
        InitializeCardCounts();
        uiManager.UpdateCardCountUI(cardCounts);
        
        // Play round
        yield return StartCoroutine(PlayRound());
        
        // Prepare for next round
        currentRound++;
        
        if (currentRound <= 3)
        {
            yield return new WaitForSeconds(1f); // Brief pause between rounds
        }
    }
    
    EndGame();
}

        public void StartBattle()
    {
        Debug.Log("=== Starting Battle ===");
        
        if (cardSelectionManager.selectedSlots.Count < 3)
        {
            Debug.LogWarning("Not all battle slots are filled!");
            return;
        }

        StopAllCoroutines();
        
        // Generate and show computer cards
        GenerateComputerCards();
        
        // Small delay before showing results
        StartCoroutine(ShowBattleResults());
    }
    private IEnumerator ShowBattleResults()
    {
        yield return new WaitForSeconds(1f);
        
        int roundScore = 0;
        Debug.Log($"=== Battle Results - Round {currentRound} ===");

        for (int i = 0; i < 3; i++)
        {
            Card playerCard = cardSelectionManager.GetCardInSlot(i);
            Card computerCard = computerCards[i];
            
            if (playerCard != null && computerCard != null)
            {
                bool isReversed = cardSelectionManager.HasReverseCard(i);
                int slotScore = battleSystem.CalculateScore(playerCard, computerCard, isReversed);
                roundScore += slotScore;
                
                string result = slotScore == 3 ? "WIN" : (slotScore == 1 ? "DRAW" : "LOSE");
                string reversedText = isReversed ? " (Reversed)" : "";
                Debug.Log($"Slot {i}: {playerCard.GetCardType()} vs {computerCard.GetCardType()} = {result} ({slotScore} points){reversedText}");
            }
            else
            {
                Debug.LogError($"Missing card in slot {i}. Player card: {playerCard != null}, Computer card: {computerCard != null}");
            }
        }

        totalScore += roundScore;
        Debug.Log($"Round Score: {roundScore}");
        Debug.Log($"Total Score: {totalScore}");
        
        uiManager.UpdateScore(totalScore);

        // Award special card if round score is high enough
        if (roundScore >= 7)
        {
            Debug.Log($"Round score {roundScore} >= 7! Awarding special card...");
            yield return StartCoroutine(AwardSpecialCard());
        }

        yield return new WaitForSeconds(2f);

        // Reset both player and computer battle slots
        cardSelectionManager.ClearBattleSlotsOnly();
        ClearComputerSlots();
        
        // Always re-enable GO button after clearing slots
        if (uiManager != null)
        {
            uiManager.EnableGoButton();
            Debug.Log("GO button re-enabled after clearing slots");
        }

        // Check round progression
        if (currentRound < 3)
        {
            currentRound++;
            Debug.Log($"Advancing to Round {currentRound}");
        }
        else
        {
            Debug.Log("Final Round Complete!");
            EndGame();
        }
    }
     private void GenerateComputerCards()
    {
        Debug.Log($"=== Generating Computer Cards for Round {currentRound} ===");

        if (deckManager == null) return;

        ClearComputerSlots();
        computerCards = new Card[3];

        // Define card range based on round
        int minIndex = 0;  // Always start with Rock
        int maxIndex;      // Max index depends on round
        
        switch (currentRound)
        {
            case 1:
                maxIndex = 3;  // Rock, Paper, Scissors only (0-2)
                Debug.Log("Round 1: Basic cards only (Rock, Paper, Scissors)");
                break;
            case 2:
                maxIndex = 4;  // Add Fire only (0-3)
                Debug.Log("Round 2: Basic cards + Fire");
                break;
            case 3:
                maxIndex = 5;  // Add Water (0-4)
                Debug.Log("Round 3: All cards except Reverse");
                break;
            default:
                maxIndex = 2;
                break;
        }

        for (int i = 0; i < computerBattleSlots.Length; i++)
        {
            if (computerBattleSlots[i] == null) continue;

            int randomIndex = Random.Range(minIndex, maxIndex);
            GameObject cardPrefab = deckManager.GetCardPrefab(randomIndex);
            if (cardPrefab == null) continue;

            Card cardComponent = cardPrefab.GetComponent<Card>();
            Image cardImage = cardPrefab.GetComponent<Image>();
            
            if (cardImage != null && cardImage.sprite != null && cardComponent != null)
            {
                computerBattleSlots[i].sprite = cardImage.sprite;
                
                Card newCard = computerBattleSlots[i].gameObject.AddComponent<Card>();
                newCard.type = cardComponent.type;
                computerCards[i] = newCard;
                
                Debug.Log($"Computer Slot {i}: {newCard.GetCardType()} (Round {currentRound})");
            }
        }
    }

private void LogComputerSlotsState()
{
    Debug.Log("=== Computer Battle Slots State ===");
    for (int i = 0; i < computerBattleSlots.Length; i++)
    {
        if (computerBattleSlots[i] == null)
        {
            Debug.LogError($"Computer slot {i} is null!");
            continue;
        }

        Transform cardTransform = computerBattleSlots[i].transform.childCount > 0 ? 
            computerBattleSlots[i].transform.GetChild(0) : null;
            
        if (cardTransform != null)
        {
            Card card = cardTransform.GetComponent<Card>();
            string cardType = card != null ? card.GetCardType().ToString() : "Unknown";
            Debug.Log($"Computer Slot {i}: {cardType}");
        }
        else
        {
            Debug.Log($"Computer Slot {i}: Empty");
        }
    }
}

        private void ResetRound()
        {
            // Clear battle slots
            cardSelectionManager.ClearAllSlots();
            
            // Clear computer slots
            ClearComputerSlots();
            
            // Clear any reverse effects
            cardSelectionManager.ClearReverseCards();
        }

        private void ClearComputerSlots()
        {
            foreach (Image slot in computerBattleSlots)
            {
                if (slot != null)
                {
                    // Remove any Card components
                    Card[] cards = slot.GetComponents<Card>();
                    foreach (Card card in cards)
                    {
                        Destroy(card);
                    }
                    
                    // Use the same empty slot sprite as player slots
                    slot.sprite = cardSelectionManager.emptySlotSprite;
                }
            }
            // Reset the computer cards array
            computerCards = new Card[3];
        }

        private IEnumerator AwardSpecialCard()
        {
            Debug.Log("Starting special card award process...");
            yield return new WaitForSeconds(0.5f);

            CardButton emptyButton = FindEmptyCardButton();
            if (emptyButton == null)
            {
                Debug.LogError("No empty card buttons found!");
                yield break;
            }

            Card specialCard = null;
            try
            {
                specialCard = deckManager.GetRandomSpecialCard();
                if (specialCard != null)
                {
                    // Make sure the button is interactable and fully visible
                    emptyButton.gameObject.SetActive(true);
                    Button button = emptyButton.GetComponent<Button>();
                    if (button != null)
                    {
                        button.interactable = true;
                    }
                    
                    // Set alpha to fully opaque
                    Image buttonImage = emptyButton.GetComponent<Image>();
                    if (buttonImage != null)
                    {
                        Color color = buttonImage.color;
                        color.a = 1f;
                        buttonImage.color = color;
                    }

                    // Set card and verify it's set correctly
                    emptyButton.SetCard(specialCard);
                    Debug.Log($"Successfully awarded special card: {specialCard.GetCardType()} to button {emptyButton.name}");

                    // Set up click handling
                    emptyButton.selectionManager = cardSelectionManager;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error creating special card: {e.Message}");
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }

        private CardButton FindEmptyCardButton()
        {
            CardButton[] buttons = FindObjectsOfType<CardButton>();
            foreach (CardButton button in buttons)
            {
                if (button.GetCard() == null && button.isActiveAndEnabled)
                {
                    Debug.Log($"Found empty card button: {button.name}");
                    return button;
                }
            }
            return null;
        }
}
}