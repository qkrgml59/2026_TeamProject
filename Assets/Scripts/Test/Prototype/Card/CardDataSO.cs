using Prototype.Unit;
using UnityEngine;

namespace Prototype.Card
{
    [CreateAssetMenu(fileName = "CardDataSO", menuName = "CardDataSO/CardDataSO")]
    public class CardDataSO : ScriptableObject
    {
        public Sprite icon;
        public string cardName;
        public string description;

        // 프로토타입에서는 유닛 카드만 존재
        public UnitBase unit;
    }
}
