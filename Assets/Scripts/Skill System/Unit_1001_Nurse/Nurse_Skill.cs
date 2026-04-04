using Prototype.Grid;
using Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unit.Skill
{
    /// <summary>
    /// Nurse 순간이동 스킬
    /// -가장 먼 적 뒤로 이동
    /// 위치 없으면 시계방향 탐색
    /// 이동 후 가장 가까운 적 기본공격
    /// </summary>
    public class Nurse_Skill : SkillBase
    {
   
        [Header("딜레이 설정")]
        [SerializeField] private float preDelayt = 0.05f;

        [Header("이펙트")]
        [SerializeField] private ParticleSystem startVFX;
        [SerializeField] private ParticleSystem arriveVFX;

        private List<ParticleSystem> spawnedVFX = new();

        private Coroutine skillRoutine;

        protected override void OnStart()
        {
            skillRoutine = StartCoroutine(SkillCast());
        }

        private IEnumerator SkillCast()
        {
            yield return new WaitForSeconds(preDelayt);

            //가장 먼 적 찾기
            UnitBase farTarget = UnitManager.Instance.FindFarthestEnemy(owner);

            if (farTarget == null )
            {
                FinishSkill();
                yield break;
            }

            //뒤 타일 혹은 시계방향 계산
            Vector3Int dir = HexMath.GetDirectionCube(owner.offset, farTarget.offset);
            Vector3Int cube = HexMath.OffsetToCube(farTarget.offset);

            HexTile tile = GridManager.Instance.GetTile(cube + dir);

            if (tile == null || !tile.CanReserve(owner))
            {
                foreach (var d in HexMath.CubeDirections)
                {
                    tile = GridManager.Instance.GetTile(cube + d);
                    if (tile != null && tile.CanReserve(owner))
                        break;
                }
            }

            if (tile == null)
            {
                FinishSkill();
                yield break;
            }
            if (!tile.TryReserve(owner))
            {
                FinishSkill();
                yield break;
            }

            //출발 이펙트
            Renderer unitRenderer = owner.GetComponentInChildren<Renderer>();
            if (unitRenderer != null)
                unitRenderer.enabled = false;

            PlayVFX(startVFX, owner.transform.position);

            yield return new WaitForSeconds(0.1f);

            // 3. 이동
            owner.EnterTile(tile);
            // TODO : UnitBase에 순간이동 기능 추가
            owner.transform.position = tile.transform.position;
           
            //이동 경로 끊기
            owner.ClearPath();

            //도착 이펙트
            if (unitRenderer != null)
                unitRenderer.enabled = true;

            Vector3 vfxPos = owner.transform.position + Vector3.up * 0.5f;
            PlayVFX(arriveVFX, vfxPos);
            yield return new WaitForSeconds(0.15f);
            //4. 타겟 설정
            owner.SetTargetUnit(farTarget);

            // 5. 종료
            FinishSkill();

        }
        private void PlayVFX(ParticleSystem prefab, Vector3 pos)
        {
            if (prefab == null) return;

            ParticleSystem fx = Instantiate(prefab, pos, Quaternion.identity);
            fx.Play();
            spawnedVFX.Add(fx);
        }

        protected override void OnCancel()
        {
            if (skillRoutine != null)
            {
                StopCoroutine(skillRoutine);
                skillRoutine = null;
            }
        }

        protected override void OnFinish()
        {
            if (skillRoutine != null)
            {
                StopCoroutine(skillRoutine);
                skillRoutine = null;
            }

            // 이펙트 정리
            foreach (var fx in spawnedVFX)
                if (fx != null)
                    Destroy(fx.gameObject);

            spawnedVFX.Clear();
        }
    }
}
