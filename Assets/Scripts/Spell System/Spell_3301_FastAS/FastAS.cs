using Prototype.Grid;
using StatSystem;
using System.Collections;
using Unit;
using UnityEngine;

namespace Spell
{
    [CreateAssetMenu(fileName = "FastASSpellSO", menuName = "SpellSO/Effects/FastASSpell")]
    public class FastAS : SpellEffectSO
    {
        [Header("자극제 버프 설정")]
        [Tooltip("지속 시간")]
        public float duration = 3f;

        [Tooltip("공격 속도 증가 값")]
        public float attackSpeedAmount = 50f;

        public override bool TryExecute(HexTile tile)
        {
            if (tile == null || tile.OccupantUnit == null) return false;

            UnitBase targetUnit = tile.OccupantUnit;

            if (BattleManager.currentBattleState != BattleState.Combat)
            {
                Debug.LogWarning("자극제는 전투 중에만 사용할 수 있습니다!");
                return false;
            }

            if (targetUnit.team != TeamType.Ally)
            {
                Debug.Log("아군 유닛에게만 사용할 수 있습니다!");
                return false;
            }

            if (targetUnit.CurFSM == UnitStateType.Dead)
            {
                Debug.Log("이미 사망한 유닛에게는 자극제를 투여할 수 없습니다!");
                return false;
            }

            targetUnit.StartCoroutine(AttackSpeedBuff(targetUnit));

            return true;
        }

        private IEnumerator AttackSpeedBuff(UnitBase unit)
        {
            StatModifier modifier = new StatModifier
            {
                statType = StatType.AttackSpeed,
                modifierType = ModifierType.Percent,
                value = attackSpeedAmount
            };

            unit.AddStatModifier(StatType.AttackSpeed, this, modifier);

            Debug.Log($"{unit.name}에게 자극제 사용 {duration}초 동안 공격속도 {attackSpeedAmount}% 증가");

            yield return new WaitForSeconds(duration);

            if (unit != null && unit.CurFSM != UnitStateType.Dead)
            {
                unit.RemoveStatModifier(this);
                Debug.Log($"[{unit.name}] 자극제 효과 종료. 공속 정상화.");
            }
        }

        public override string GetDescription()
        {
            return $"타겟 대상의 공격 속도를 {duration}초간 {attackSpeedAmount}% 만큼 증가시킵니다.";
        }
    }
}
