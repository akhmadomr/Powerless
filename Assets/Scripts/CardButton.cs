using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Powerless.UI; 
using Powerless.Core;

namespace Powerless.UI
{
    public class CardButton : MonoBehaviour
    {
        public int cardIndex;
        public CardSelectionManager selectionManager;
        private Card card;
        private Image buttonImage;
        [SerializeField] private Sprite defaultSprite; // Add this for default sprite

        private void Awake()
        {
            buttonImage = GetComponent<Image>();
            if (buttonImage != null && defaultSprite != null)
            {
                buttonImage.sprite = defaultSprite;
            }
        }

        public void SetCard(Card newCard)
        {
            card = newCard;
            if (card != null && buttonImage != null)
            {
                // Get sprite from card prefab
                Image cardImage = card.GetComponent<Image>();
                if (cardImage != null)
                {
                    buttonImage.sprite = cardImage.sprite;
                    buttonImage.color = Color.white; // Full opacity
                    Debug.Log($"Set card {cardIndex} image to {card.GetCardType()}");
                }
            }
        }

        public void ClearCard()
        {
            if (card != null)
            {
                Debug.Log($"Clearing card {card.GetCardType()} from button {cardIndex}");
                card = null;
                if (buttonImage != null)
                {
                    buttonImage.sprite = defaultSprite; // Use default sprite instead of null
                    buttonImage.color = Color.white; // Reset to full opacity
                }
            }
        }

        public Card GetCard()
        {
            return card;
        }

        public void OnButtonClick()
        {
            if (card != null)
            {
                selectionManager.OnCardSelected(cardIndex);
                Debug.Log($"Clicked card {cardIndex} of type {card.GetCardType()}");
            }
        }
    }
}