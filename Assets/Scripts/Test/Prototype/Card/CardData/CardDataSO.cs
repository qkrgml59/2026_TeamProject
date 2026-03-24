using Prototype.Unit;
using UnityEngine;

namespace Prototype.Card
{
    public class CardDataSO : ScriptableObject
    {
        public string cardId;

        public Sprite icon;
        public string cardName;
        public string description;

        public CardBase cardPrefab;
    }
}
