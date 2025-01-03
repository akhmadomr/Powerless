using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Powerless.Core;
using Powerless.UI;

namespace Powerless.UI
{
public class CardSelectionManager : MonoBehaviour
{
    public Button[] cardButtons;
    public Image[] battleSlots;
    [SerializeField] public Sprite emptySlotSprite; // Make it public and serializable
    
    public List<int> selectedSlots = new List<int>();
    public Dictionary<int, int> slotToCardIndex = new Dictionary<int, int>();
    private Dictionary<int, Sprite> originalCardSprites = new Dictionary<int, Sprite>();
    private Dictionary<int, bool> slotReversedStatus = new Dictionary<int, bool>();

    void Awake()
    {
        // Initialize dictionaries
        originalCardSprites = new Dictionary<int, Sprite>();
        slotToCardIndex = new Dictionary<int, int>();
        selectedSlots = new List<int>();
        cardCounts = new Dictionary<Card.CardType, int>();
    }

    void Start()
    {
        if (cardButtons == null || cardButtons.Length == 0)
        {
            Debug.LogError("Card buttons array is null or empty!");
            return;
        }

        if (battleSlots == null || battleSlots.Length == 0)
        {
            Debug.LogError("Battle slots array is null or empty!");
            return;
        }

        // Store original card sprites
        for (int i = 0; i < cardButtons.Length; i++)
        {
            if (cardButtons[i] != null)
            {
                Image buttonImage = cardButtons[i].GetComponent<Image>();
                if (buttonImage != null && buttonImage.sprite != null)
                {
                    originalCardSprites[i] = buttonImage.sprite;
                }
            }
        }

        // Initialize card buttons
        for (int i = 0; i < cardButtons.Length; i++)
        {
            if (cardButtons[i] != null)
            {
                int index = i;
                cardButtons[i].onClick.AddListener(() => OnCardSelected(index));
            }
        }

        // Initialize battle slots
        for (int i = 0; i < battleSlots.Length; i++)
        {
            if (battleSlots[i] != null && emptySlotSprite != null)
            {
                battleSlots[i].sprite = emptySlotSprite;
            }
        }
        
        LogInitialCards();
        InitializeBattleSlots();
    }
    private void LogInitialCards()
    {
        Debug.Log("=== Initial Cards ===");
        if (cardButtons == null)
        {
            Debug.LogError("Card buttons array is null!");
            return;
        }

        for (int i = 0; i < cardButtons.Length; i++)
        {
            if (cardButtons[i] == null)
            {
                Debug.LogWarning($"Card button {i} is null");
                continue;
            }

            CardButton cardButton = cardButtons[i].GetComponent<CardButton>();
            if (cardButton == null)
            {
                Debug.LogWarning($"No CardButton component on button {i}");
                continue;
            }

            Card card = cardButton.GetCard();
            if (card != null)
            {
                Debug.Log($"Card {i}: {card.GetCardType()}");
            }
            else
            {
                Debug.LogWarning($"No Card assigned to button {i}");
            }
        }
    }


    public void OnCardSelected(int cardIndex)
    {
        CardButton cardButton = cardButtons[cardIndex].GetComponent<CardButton>();
        if (cardButton == null || cardButton.GetCard() == null)
        {
            return;
        }

        int emptySlotIndex = -1;
        for (int i = 0; i < battleSlots.Length; i++)
        {
            if (!selectedSlots.Contains(i))
            {
                emptySlotIndex = i;
                break;
            }
        }

        if (emptySlotIndex != -1)
        {
            MoveCardToSlot(cardIndex, emptySlotIndex);
        }
    }

    public void OnBattleSlotClicked(int slotIndex)
    {
        Debug.Log($"Battle slot {slotIndex} clicked");
        if (slotToCardIndex.ContainsKey(slotIndex))
        {
            ReturnCardToButton(slotIndex);
        }
    }

    private void MoveCardToSlot(int cardIndex, int slotIndex)
    {
        CardButton cardButton = cardButtons[cardIndex].GetComponent<CardButton>();
        Card card = cardButton.GetCard();
        
        if (card != null)
        {
            selectedCards[slotIndex] = card;
            battleSlots[slotIndex].sprite = cardButtons[cardIndex].GetComponent<Image>().sprite;
            selectedSlots.Add(slotIndex);
            slotToCardIndex[slotIndex] = cardIndex;
            cardButton.ClearCard();
            cardButtons[cardIndex].interactable = false;
        }
    }

    private void ReturnCardToButton(int slotIndex)
    {
        if (slotToCardIndex.TryGetValue(slotIndex, out int cardIndex))
        {
            CardButton cardButton = cardButtons[cardIndex].GetComponent<CardButton>();
            Card slotCard = selectedCards[slotIndex];
            
            if (slotCard != null)
            {
                cardButton.SetCard(slotCard);
                cardButtons[cardIndex].interactable = true;
                battleSlots[slotIndex].sprite = emptySlotSprite;
                selectedSlots.Remove(slotIndex);
                slotToCardIndex.Remove(slotIndex);
                selectedCards.Remove(slotIndex);
            }
        }
    }

    // Add this method to initialize battle slot click events
    private void InitializeBattleSlots()
    {
        for (int i = 0; i < battleSlots.Length; i++)
        {
            int index = i;
            Button slotButton = battleSlots[i].GetComponent<Button>();
            if (slotButton != null)
            {
                slotButton.onClick.AddListener(() => OnBattleSlotClicked(index));
            }
            else
            {
                Debug.LogWarning($"Battle slot {i} missing Button component!");
            }
        }
    }

    private void LogCardSelection(int cardIndex)
    {
        if (cardIndex < 0 || cardIndex >= cardButtons.Length)
        {
            Debug.LogError($"Invalid card index: {cardIndex}");
            return;
        }

        CardButton cardButton = cardButtons[cardIndex].GetComponent<CardButton>();
        if (cardButton == null)
        {
            Debug.LogError($"No CardButton component found on button {cardIndex}");
            return;
        }

        Card card = cardButton.GetCard();
        if (card == null)
        {
            Debug.LogError($"No Card assigned to CardButton {cardIndex}");
            return;
        }

        Debug.Log($"Selected Card: Index={cardIndex}, Type={card.GetCardType()}");
    }

    private void LogBattleSlotState()
    {
        Debug.Log("=== Battle Slots State ===");
        
        if (battleSlots == null)
        {
            Debug.LogError("Battle slots array is null!");
            return;
        }

        for (int i = 0; i < battleSlots.Length; i++)
        {
            if (battleSlots[i] == null)
            {
                Debug.LogWarning($"Battle slot {i} is null");
                continue;
            }

            if (slotToCardIndex.ContainsKey(i))
            {
                int cardIndex = slotToCardIndex[i];
                if (cardIndex >= 0 && cardIndex < cardButtons.Length && cardButtons[cardIndex] != null)
                {
                    CardButton cardButton = cardButtons[cardIndex].GetComponent<CardButton>();
                    if (cardButton != null)
                    {
                        Card card = cardButton.GetCard();
                        if (card != null)
                        {
                            Debug.Log($"Slot {i}: {card.GetCardType()}");
                        }
                        else
                        {
                            Debug.LogWarning($"Slot {i}: Card component not found");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Slot {i}: CardButton component not found");
                    }
                }
                else
                {
                    Debug.LogWarning($"Slot {i}: Invalid card index {cardIndex}");
                }
            }
            else
            {
                Debug.Log($"Slot {i}: Empty");
            }
        }
    }

    public void ClearSlot(int slotIndex, bool reenableButton = true)
    {
        if (slotToCardIndex.ContainsKey(slotIndex))
        {
            int cardIndex = slotToCardIndex[slotIndex];
            
            if (reenableButton)
            {
                cardButtons[cardIndex].interactable = true;
            }
            
            battleSlots[slotIndex].sprite = emptySlotSprite;
            slotToCardIndex.Remove(slotIndex);
            selectedSlots.Remove(slotIndex);
        }
    }

    private Dictionary<Card.CardType, int> cardCounts = new Dictionary<Card.CardType, int>();
    
    public void UpdateCardCounts()
    {
        foreach(Card.CardType type in System.Enum.GetValues(typeof(Card.CardType)))
        {
            cardCounts[type] = 0;
        }
        
        foreach(Button cardButton in cardButtons)
        {
            Card card = cardButton.GetComponent<Card>();
            if(card != null)
            {
                cardCounts[card.type]++;
            }
        }
        
        UpdateCountUI();
    }
    public Card GetCardInSlot(int slotIndex)
    {
        // First try to get from selectedCards dictionary
        if (selectedCards.ContainsKey(slotIndex))
        {
            return selectedCards[slotIndex];
        }
        
        // Fallback to getting from button if needed
        if (slotToCardIndex.ContainsKey(slotIndex))
        {
            int cardIndex = slotToCardIndex[slotIndex];
            return cardButtons[cardIndex].GetComponent<CardButton>().GetCard();
        }
        return null;
    }
    private void UpdateCountUI()
        {
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.UpdateCardCountUI(cardCounts);
            }
            else
            {
                Debug.LogError("UIManager not found!");
            }
        }

        public Dictionary<Card.CardType, int> GetCardCounts()
        {
            return cardCounts;
        }

    public void ApplyReverseCard(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < battleSlots.Length)
        {
            slotReversedStatus[slotIndex] = true;
            Debug.Log($"Applied reverse card to slot {slotIndex}");
        }
    }

    public bool HasReverseCard(int slotIndex)
    {
        return slotReversedStatus.ContainsKey(slotIndex) && slotReversedStatus[slotIndex];
    }

    public void ClearReverseCards()
    {
        slotReversedStatus.Clear();
    }

    public void ClearAllSlots()
    {
        List<int> slotsToClean = new List<int>(selectedSlots);
        foreach (int slotIndex in slotsToClean)
        {
            ClearSlot(slotIndex);
        }
        selectedSlots.Clear();
        slotToCardIndex.Clear();
    }

    public void ClearBattleSlotsOnly()
    {
        // Clear battle slots without re-enabling card buttons
        foreach (int slotIndex in selectedSlots)
        {
            if (battleSlots[slotIndex] != null)
            {
                battleSlots[slotIndex].sprite = emptySlotSprite;
            }
        }
        
        selectedSlots.Clear();
        slotToCardIndex.Clear();
        selectedCards.Clear(); // Add this line
        
        Debug.Log("Cleared battle slots only");
    }

    // Add new dictionary to track cards in slots
    private Dictionary<int, Card> selectedCards = new Dictionary<int, Card>();
}
}