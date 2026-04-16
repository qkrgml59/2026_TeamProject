using Prototype.Card;
using Prototype.Grid;
using Unit;
using UnityEngine;

namespace Spell
{
    [CreateAssetMenu(fileName = "RunAwayEffect", menuName = "SpellSO/Effects/RunAway")]
    public class RunAway :SpellEffectSO
    {
        public override bool TryExecute(HexTile tile)
        {
            if (BattleManager.Instance.currentBattleState != BattleState.Combat)
            {
                Debug.LogWarning("도망가기는 전투 중에만 사용할 수 있습니다!");
                return false;
            }

            if (tile == null || tile.OccupantUnit == null)
            {
                Debug.LogWarning("해당 타일에 유닛이 없습니다.");
                return false;
            }

            UnitBase targetUnit = tile.OccupantUnit;

            if (targetUnit.team != TeamType.Ally) return false;

            CardDataSO returnCardData = targetUnit.originCardData;

            if (returnCardData == null)
            {
                Debug.LogError($"[{targetUnit.name}] 유닛에 원본 카드 데이터(originCardData)가 없습니다!");
                return false;
            }

            BattleCardManager.Instance.AddBattleCard(returnCardData, BattleCardZone.Hand);          // 손패로 되돌리기

            targetUnit.Die();           // 오브젝트 파괴 및 UnitManager에서 제거

            Debug.Log($"사용 성공! {targetUnit.name}을(를) 손패로 되돌렸습니다!");
            return true;
        }

        public override string GetDescription()
        {
            return "타겟 아군을 필드에서 즉시 제거 후 카드화하여 손패로 되돌립니다.";
        }
    }
}
