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

        private void Awake()
        {
            buttonImage = GetComponent<Image>();
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
                    buttonImage.sprite = null;
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