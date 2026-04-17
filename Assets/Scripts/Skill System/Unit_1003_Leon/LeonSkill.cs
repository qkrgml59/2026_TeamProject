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
        [Header("돌진 범위")]
        [SerializeField] private int dashDistance = 2;

        [Header("딜레이")]
        [SerializeField] private float preDelay = 0.05f;
        [SerializeField] private float dashDuration = 0.5f;
        private bool dashStart = false;

        private Coroutine skillRoutine;
        protected override void OnStart()
        {
            dashStart = false;
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
            dashStart = true;
            yield return Dash(moveTile);
            

            FinishSkill();
        }

        /// <summary>
        /// 대쉬 연출
        /// </summary>
        private IEnumerator Dash(HexTile tile)
        {
            if (tile == null)
                yield break;
                
            if(!tile.TryReserve(owner))
                yield break;

            Vector3 startPos = owner.transform.position;
            Vector3 targetPos = tile.transform.position;
            float elapsed = 0f;

            while (elapsed < dashDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / dashDuration);

                owner.transform.position = Vector3.Lerp(startPos, targetPos, t);
                yield return null;
            }

            owner.transform.position = targetPos;
            owner.SetTile(tile);
            owner.ClearPath();
        }

        ///<summary>
        ///공격 사거리 내 가장 먼 타일 탐색
        /// </summary>
        private HexTile FindFarthestTileInRange()
        {
            if (owner.targetUnit == null)
            {
                var target = UnitManager.Instance.GetNearestEnemy(owner);
                if (target == null)
                    return null;

                owner.SetTargetUnit(target);
            }

            int range = Mathf.RoundToInt(owner.statSet.AttackRange.Value);
            Vector3Int start = HexMath.OffsetToCube(owner.offset);

            HexTile bestTile = null;
            int maxDist = -1;

            foreach (var dir in HexMath.CubeDirections)
            {
                // 6방향으로 대쉬 거리 만큼 타일 탐색
                for (int i = 1; i <= dashDistance; i++)
                {
                    HexTile tile = GridManager.Instance.GetTile(start + dir * i);

                    if (tile == null || !tile.CanReserve(owner)) break;     // 지나갈 수 없는 방향은 무시

                    int dist = HexMath.Distance(tile.Offset, owner.targetUnit.offset);
                    if (dist > range) continue;  // 도착 위치에서 타겟을 못 때리면 넘어가기
                        

                    if (dist > maxDist)
                    {
                        maxDist = dist;
                        bestTile = tile;
                    }
                }
            }
            return bestTile;
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
            if(dashStart) return;       // 이미 대쉬가 시작 되었으면 계속 진행

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


