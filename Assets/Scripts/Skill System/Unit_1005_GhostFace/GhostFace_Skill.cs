using Prototype.Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.Grid;

namespace Unit.Skill
{
    public class GhostFace_Skill : SkillBase
    {
        [Header("스킬 수치")]
        [SerializeField] private float skillDamage = 100f;
        [SerializeField] private float Value_A = 0.8f;

        [Header("이동 속도 임시 추가")]
        [SerializeField] private float dashSpeed = 5f;

        [Header("딜레이")]
        [SerializeField] private float preDelay = 0.05f;
        [SerializeField] private float hitDelay = 0.1f;

        [Header("이펙트")]
        [SerializeField] private ParticleSystem startVFX;
        [SerializeField] private ParticleSystem hitVFX;

        private List<ParticleSystem> spawnedVFX = new();
        private Coroutine skillRoutine;

        protected override void OnStart()
        {
            Debug.Log("GhostFace Skill Start");
            skillRoutine = StartCoroutine(SkillCast());
        }

        private IEnumerator SkillCast()
        {
            yield return new WaitForSeconds(preDelay);

            //1. 무조건 은신 먼저
            SetStealthVisual(true);
            PlayVFX(startVFX, owner.transform.position);

            //2. 타겟 찾기
            UnitBase target = FindLowestHpEnemy();

            if (target == null)
            {
                SetStealthVisual(false);
                FinishSkill();
                yield break;
            }

            owner.SetTargetUnit(target);

            //3. 이동 시도
            HexTile dashTile = FindDashTile(target);

            if (dashTile != null)
            {
                yield return Dash(dashTile);
            }
            else
            {
                Debug.Log("[고스트페이스]: 경로 없음, 제자리 공격");
            }

            yield return new WaitForSeconds(hitDelay);

            //4. 타격
            if (target != null)
            {
                PlayVFX(hitVFX, target.transform.position);
                target.ApplyDamage(GetDamage());
            }

            //5. 은신 해제
            SetStealthVisual(false);

            FinishSkill();

        }

        //체력 가장 낮은 유닛에게 이동
        private UnitBase FindLowestHpEnemy()
        {
            var enemies = UnitManager.Instance.GetAliveEnemies(owner.team);

            UnitBase target = null;
            float minHp = float.MaxValue;

            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;

                float hp = enemy.currentHp;

                if (hp < minHp)
                {
                    minHp = hp;
                    target = enemy;
                }
            }

            return target;
        }

        private IEnumerator Dash(HexTile tile)
        {
            if (!tile.TryReserve(owner))
                yield break;

            Vector3 start = owner.transform.position;
            Vector3 end = tile.transform.position;

            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * dashSpeed;
                owner.transform.position = Vector3.Lerp(start, end, t);
                yield return null;
            }

            owner.transform.position = end;
            owner.EnterTile(tile);
        }

        private HexTile FindDashTile(UnitBase target)
        {
            Vector3Int targetCube = HexMath.OffsetToCube(target.offset);

            HexTile bestTile = null;

            // 시계 방향 거리 계산
            for (int i = 0; i < HexMath.CubeDirections.Length; i++)
            {
                Vector3Int dir = HexMath.CubeDirections[i];

                for (int step = 1; step <= 2; step++)
                {
                    Vector3Int checkPos = targetCube + dir * step;

                    HexTile tile = GridManager.Instance.GetTile(checkPos);

                    if (tile == null)
                        continue;

                    if (!tile.CanReserve(owner))
                        continue;

                    //타겟과 너무 멀어지지 않게 제한
                    int dist = HexMath.Distance(tile.offset, target.offset);

                    if (dist > owner.statSet.AttackRange.Value + 1)
                        continue;

                    bestTile = tile;
                    return bestTile; 
                }
            }

            return null;
        }

        private DamageInfo GetDamage()
        {
            float dmg = skillDamage;
            dmg += owner.statSet.AttackDamage.Value * Value_A;

            dmg *= (100 + owner.statSet.DamageIncrease.Value) * 0.01f;

            return new DamageInfo(owner, dmg, DamageType.Physical, false);
        
        }

        //은신
        private void SetStealthVisual(bool isStealth)
        {
            var renderers = owner.GetComponentsInChildren<Renderer>();

            foreach(var r in renderers)
            {
                if (r == null) continue;
                r.enabled = !isStealth;
            }
        }

        private void PlayVFX(ParticleSystem prefab, Vector3 pos)
        {
            if (prefab == null) return;

            var fx = Instantiate(prefab, pos, Quaternion.identity);
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

            foreach (var fx in spawnedVFX)
                if (fx != null)
                    Object.Destroy(fx.gameObject);

            spawnedVFX.Clear();
        }
    }
}

