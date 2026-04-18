using Prototype.Grid;
using Unit;
using UnityEngine;

namespace Spell
{
    [CreateAssetMenu(fileName = "HealUnitSO", menuName = "SpellSO/Effects/HealUnit")]
    public class HealUnit : SpellEffectSO
    {
        [Header("회복 수치 설정")]
        public float healAmount = 15f;
        public override bool TryExecute(HexTile tile)
        {
            if (tile == null || tile.OccupantUnit == null)
            {
                Debug.LogWarning("해당 타일에 유닛이 없습니다.");
                return false;
            }

            UnitBase targetUnit = tile.OccupantUnit;

            if (targetUnit.team != TeamType.Ally)
            {
                Debug.LogWarning("아군 유닛에게만 사용할 수 있습니다.");
                return false;
            }

            if (targetUnit.statSet == null || targetUnit.statSet.MaxHp == null)
            {
                Debug.LogError($"{targetUnit.name} 유닛의 체력(Stat) 데이터가 없습니다!");
                return false;
            }

            float maxHp = targetUnit.statSet.MaxHp.Value;

            float _healAmount = maxHp * (healAmount / 100f);

            HealInfo healInfo = new HealInfo(targetUnit, _healAmount);
            targetUnit.ApplyHeal(healInfo);

            Debug.Log($"[붕대 감기] {targetUnit.name} 체력 {_healAmount} 회복 (최대 체력의 {healAmount}%)");
            return true;
        }


        public override string GetDescription()
        {
            return $"타겟 대상의 현재 체력을 최대 체력의 {healAmount}%만큼 즉시 회복시킵니다";
        }
    }
}
