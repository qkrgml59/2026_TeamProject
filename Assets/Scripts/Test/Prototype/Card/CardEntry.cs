using Prototype.Card;
using UnityEngine;

[System.Serializable]
public class CardEntry
{
    public string cardId => cardData ? cardData.cardId : null;
    public CardDataSO cardData;
    public int count;

    public CardEntry(CardDataSO cardData, int count = 1)
    {
        this.cardData = cardData;
        this.count = count;
    }
}
