using Unit;
using UnityEngine;

namespace Prototype.Card
{
    public abstract class CardDataSO : ScriptableObject
    {
        public string cardId;

        public Sprite icon;
        public string cardName;
        public string description;

        public CardBase cardPrefab;

        /// <summary>
        /// 카드의 종류 (유닛, 스펠, 아이템) 반환
        /// </summary>
        public abstract CardType CardType { get; }
    }
}
