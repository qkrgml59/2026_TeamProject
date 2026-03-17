using Prototype.Card;
using UnityEngine;

[System.Serializable]
public class CardEntry : MonoBehaviour
{
    public string cardId;
    public CardDataSO cardData;
    public int count;

    public CardEntry(CardDataSO cardData, int count = 1)
    {
        cardId = cardData.cardId;
        this.cardData = cardData;
        this.count = count;
    }
}
