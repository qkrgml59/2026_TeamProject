using Prototype.Card;
using Unit;
using UnityEngine;

namespace Prototype.Card.Unit
{
    [CreateAssetMenu(fileName = "CardDataSO", menuName = "CardDataSO/UnitCardDataSO")]
    public class UnitCardDataSO : CardDataSO
    {
        public UnitBase unitSO;
    }
}
