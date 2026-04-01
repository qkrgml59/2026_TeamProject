using Prototype.Card;
using Unit;
using UnityEngine;

namespace Prototype.Card.Unit
{
    [CreateAssetMenu(fileName = "CardDataSO", menuName = "CardDataSO/UnitCardDataSO")]
    public class UnitCardDataSO : CardDataSO
    {
        public UnitStatSO unitStatSO;
        public UnitBase unitSO; //unitStatSO로 관리되어 사용하게 될 때 삭제. 현재 프리팹으로 테스팅중이기에 남겨둠.
    }
}
