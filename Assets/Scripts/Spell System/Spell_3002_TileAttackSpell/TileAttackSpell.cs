using Prototype.Grid;
using UnityEngine;

namespace Spell.Effects
{
    [CreateAssetMenu(fileName = "TileAttackSpell", menuName = "SpellSO/Effects/TileAttackSpell")]
    public class TileAttackSpell : SpellEffectSO
    {
        [Header("세팅")]
        public float damage = 10f;
        [Tooltip("DamageType(물리,마법,고정)")] public DamageType damageType = DamageType.None;       //기본 값은 고정뎀
        //이펙트
        //public GameObject projectilePrefab;       이펙티 추가할 때.

        public override bool TryExecute(RaycastHit hit)
        {
            // 전투 중이 아닐 때의 예외 처리
            if (BattleManager.Instance.currentBattleState != BattleState.Combat)
            {
                Debug.LogWarning("전투가 시작된 후에만 사용할 수 있습니다!");
                return false;
            }

            HexTile tile = hit.transform.GetComponent<HexTile>();

            if (tile != null)
            {
                //타일 위에 유닛이 있을 때
                if (tile.OccupantUnit != null)
                {
                    // 아군에게 사용 시 예외 처리
                    if (tile.OccupantUnit.team == Unit.TeamType.Ally)
                    {
                        Debug.LogWarning("아군 유닛에게는 공격 마법을 사용할 수 없습니다!");
                        return false;
                    }

                    DamageInfo info = new DamageInfo(
                        caster : null,
                        amount : damage,
                        damageType : damageType,
                        isCritical : false
                    );

                    tile.OccupantUnit.ApplyDamage(info);

                    Debug.Log($"카드 사용 {tile.OccupantUnit.name}에게 {damage}의 피해를 입힘.");

                    /*      //이펙트 추가 시.
                    if (projectilePrefab != null)
                    {
                        Instantiate(projectilePrefab, tile.transform.position, Quaternion.identity);
                    }   */

                    return true;
                }
                return false;
            }

            Debug.LogWarning("타일을 지정하세요.");
            return false;
        }

        public override string GetDescription() => $"지정한 타일에 {damage} 만큼의 {damageType}피해를 입힙니다.";
    }
}

