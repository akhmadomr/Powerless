using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Powerless.Core
{
public class Card : MonoBehaviour
{
    public enum CardType { Rock, Paper, Scissors, Fire, Water, Reverse }
    
    [SerializeField] public CardType type;
    [SerializeField] private Sprite cardSprite;
    [SerializeField] private int powerValue = 1;
    
    public CardType GetCardType() => type;
    public int GetPowerValue() => powerValue;
}
}