using Prototype.Grid;
using System.Collections;
using Unit;
using UnityEngine;

namespace Spell
{
    [CreateAssetMenu(fileName = "StunSpellSO", menuName = "SpellSO/Effects/StunSpell")]
    public class StunSpell : SpellEffectSO
    {
        [Header("기절시간 설정")]
        public float stunDuration = 1.5f;
        public override bool TryExecute(HexTile tile)
        {
            if (tile == null || tile.OccupantUnit == null)
            {
                Debug.LogWarning("해당 타일에 유닛이 없습니다.");
                return false;
            }

            UnitBase targetUnit = tile.OccupantUnit;

            if (targetUnit.team != TeamType.Enemy)
            {
                Debug.LogWarning("적 유닛에게만 사용할 수 있습니다.");
                return false;
            }

            // 기절 추가시 주석 풀기
            // targetUnit.StartCoroutine(StunCoroutines(targetUnit));                // targetUnit에서 코루틴 실행 => FSM Stun을 들고있어서

            Debug.Log($"[몽둥이질] {targetUnit.name}이/가 {stunDuration}초 동안 기절합니다.");
            return true;
        }

        private IEnumerator StunCoroutines(UnitBase unit)
        {
            unit.ChangeUnitState(UnitStateType.Stun);                               // 유닛의 FSM을 Stun으로 강제 변경

            yield return new WaitForSeconds(stunDuration);                          // 기절시간 만큼 대기

            if (unit != null && unit.CurFSM == UnitStateType.Stun)                  // Unit이 살아있고 여전히 기절 상태일 때
            {
                unit.ChangeUnitState(UnitStateType.Think);
                Debug.Log($"{unit.name} 기절 종료.");
            }
        }

        public override string GetDescription()
        {
            return $"적 단일 대상에게 {stunDuration}초간 기절 상태이상을 부여합니다.";
        }
    }
}
