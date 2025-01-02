using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Powerless.Core;
using TMPro; 

namespace Powerless.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI[] cardCountTexts;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Button goButton;
        [SerializeField] private GameManager gameManager;

        void Start()
    {
        if (gameManager == null)
        {
            gameManager = GetComponent<GameManager>();
        }
        gameManager = FindObjectOfType<GameManager>();
        if (goButton != null)
        {
            goButton.onClick.AddListener(OnGoButtonClicked);
            goButton.interactable = true;
        }
    }

public void UpdateTimer(float time)
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(time);
            timerText.text = $"Time: {seconds}";
            
            // Change color when time is low
            if (seconds <= 5)
            {
                timerText.color = Color.red;
            }
        }
    }
        
        public void UpdateCardCount(Card.CardType type, int count)
        {
            int index = (int)type;
            if(index < cardCountTexts.Length)
            {
                cardCountTexts[index].text = $"x{count}";
            }
        }

        public void UpdateCardCountUI(Dictionary<Card.CardType, int> counts)
        {
            foreach(var pair in counts)
            {
                UpdateCardCount(pair.Key, pair.Value);
            }
        }
        
        public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
            Debug.Log($"Updated score display: {score}");
        }
    }
        private void OnGoButtonClicked()
    {
        Debug.Log("Go Button Clicked - Starting Battle!");
        goButton.interactable = false;
        gameManager.StartBattle();
    }

    public void EnableGoButton()
    {
        if (goButton != null)
        {
            goButton.interactable = true;
            Debug.Log("GO button re-enabled");
        }
    }
    }
}