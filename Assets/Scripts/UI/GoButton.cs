using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Powerless.Core;

namespace Powerless.UI
{
    public class GoButton : MonoBehaviour
    {
        private Button button;
        private GameManager gameManager;

        void Start()
        {
            button = GetComponent<Button>();
            gameManager = FindObjectOfType<GameManager>();
            
            if (button != null)
            {
                button.onClick.AddListener(OnClick);
            }
        }

        private void OnClick()
        {
            button.interactable = false;
            gameManager.StartBattle();
        }
    }
}