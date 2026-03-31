using Prototype.Grid;
using System.Collections;
using Unit.Skill;
using UnityEngine;

namespace Unit.Skill
{
    ///<summary>
    ///타겟 반대 방향으로 최대 사거리까지 도약
    ///이동 중 무적
    ///뒤에 거리가없을 시 옆으로 이동
    /// </summary>
    public class LeonSkill : SkillBase
    {
        [Header("딜레이")]
        [SerializeField] private float preDelay = 0.05f;

        private Coroutine skillRoutine;
        protected override void OnStart()
        {
            skillRoutine = StartCoroutine(SkillCast());
        }

        private IEnumerator SkillCast()
        {
            yield return new WaitForSeconds(preDelay);

            //1. 이동할 타일 찾기
            HexTile moveTile = FindFarthestTileInRange();

            if (moveTile == null)
            {
                FinishSkill();
                yield break;

            }

            //2. 이동
            TeleportTo(owner, moveTile);

            FinishSkill();



        }

        ///<summary>
        ///공격 사거리 내 가장 먼 타일 탐색
        /// </summary>
        private HexTile FindFarthestTileInRange()
        {
            int maxRange = Mathf.RoundToInt(owner.statSet.AttackRange.Value);
            Vector3Int start = HexMath.OffsetToCube(owner.offset);

            HexTile bestTile = null;
            int maxDist = -1;

            foreach (var dir in HexMath.CubeDirections)
            {
                for (int i = 1; i <= maxRange; i++)
                {
                    HexTile tile = GridManager.Instance.GetTile(start + dir * i);
                    if (tile == null || !tile.CanReserve(owner))
                    {
                        break;
                    }

                    if (i > maxDist)
                    {
                        maxDist = i;
                        bestTile = tile;
                    }
                }

                

            }
            return bestTile;
        }

        private void TeleportTo(UnitBase unit, HexTile tile)
        {
            if (unit == null || tile == null)
                return;

            unit.EnterTile(tile);
            unit.transform.position = tile.transform.position;

            unit.ClearPath();

        }

        private void SetInvincible(bool value)
        {
            // 현재는 물리 충돌 막는 방식
            owner.Kinematic(value);

            // 
            // owner.isInvincible = value;
        }

        protected override void OnCancel()
        {
            if(skillRoutine != null)
            {
                StopCoroutine(skillRoutine);
                skillRoutine = null;
            }
        }

        protected override void OnFinish()
        {
            if (skillRoutine !=null)
            {
                StopCoroutine(skillRoutine);
                skillRoutine = null;
            }
        }

    }
}


