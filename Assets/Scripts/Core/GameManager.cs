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
    [SerializeField] private float roundTime = 30f; 
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Image[] computerBattleSlots = new Image[3];
    
    [SerializeField] private CardSelectionManager cardSelectionManager;
    
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
            foreach (Image slot in computerBattleSlots)
            {
                if (slot != null) slot.sprite = null;
            }
            
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
            float timeLeft = roundTime;
            bool roundEnded = false;

            while (timeLeft > 0 && !roundEnded)
            {
                timeLeft -= Time.deltaTime;
                uiManager.UpdateTimer(timeLeft);
                
                // Check if player finished selecting cards
                if (timeLeft <= 0)
                {
                    roundEnded = true;
                }
                
                yield return null;
            }

            // Calculate round score
            CalculateRoundScore();
            
            // Check if special card should be awarded
            if (totalScore >= 7)
            {
                Card specialCard = deckManager.GetRandomSpecialCard();
                // Add to player's deck
            }

            yield return new WaitForSeconds(2f); // Show results
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

        // Reset battle slots without returning cards
        cardSelectionManager.ClearBattleSlotsOnly();
        
        // Re-enable GO button
        if (uiManager != null)
        {
            uiManager.EnableGoButton();
        }

        if (currentRound < 3)
        {
            currentRound++;
        }
        else
        {
            EndGame();
        }
    }
     private void GenerateComputerCards()
    {
        Debug.Log($"=== Generating Computer Cards for Round {currentRound} ===");

        if (deckManager == null) return;

        ClearComputerSlots();
        computerCards = new Card[3];

        for (int i = 0; i < computerBattleSlots.Length; i++)
        {
            if (computerBattleSlots[i] == null) continue;

            // Generate index for allowed card types only
            int randomIndex;
            if (currentRound == 1)
            {
                // Round 1: Only Rock (0), Paper (1), Scissors (2)
                randomIndex = Random.Range(0, 3);
            }
            else
            {
                // Round 2-3: Only Rock (0), Paper (1), Scissors (2), Fire (3), Water (4)
                // Skip Reverse (5)
                do
                {
                    randomIndex = Random.Range(0, 5);
                } while (randomIndex >= 5); // Make sure we never get index 5 (Reverse)
            }

            GameObject cardPrefab = deckManager.GetCardPrefab(randomIndex);
            if (cardPrefab == null) continue;

            GameObject cardObj = Instantiate(cardPrefab, computerBattleSlots[i].transform);
            cardObj.transform.localPosition = Vector3.zero;
            cardObj.transform.localScale = Vector3.one;
            
            Card card = cardObj.GetComponent<Card>();
            Image cardImage = cardObj.GetComponent<Image>();
            
            if (cardImage != null && cardImage.sprite != null && card != null)
            {
                // Verify the card is not a Reverse card before assigning
                if (card.GetCardType() != Card.CardType.Reverse)
                {
                    computerBattleSlots[i].sprite = cardImage.sprite;
                    computerCards[i] = card;
                    Debug.Log($"Computer Slot {i}: {card.GetCardType()}");
                }
                else
                {
                    Debug.LogWarning($"Prevented Reverse card generation in slot {i}");
                    Destroy(cardObj);
                    i--; // Retry this slot
                }
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
            // Reset timer
            uiManager.UpdateTimer(roundTime);
            
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
                    foreach (Transform child in slot.transform)
                    {
                        Destroy(child.gameObject);
                    }
                    slot.sprite = null;
                }
            }
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