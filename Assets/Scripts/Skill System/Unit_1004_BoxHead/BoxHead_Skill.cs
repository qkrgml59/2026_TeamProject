using Prototype.Grid;
using StatSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unit.Skill
{
    public class BoxHead_Skill : SkillBase
    {
        [Header("효과 수치")]
        [SerializeField] private float baseDamage = 100;
        [SerializeField] private float ValueA = 1.5f;
        [SerializeField] private float ValueB = 2f;
        [SerializeField] private int TargetCount = 3;

        [Header("딜레이 설정")]
        [SerializeField] private float preDelay = 0.05f;
        [SerializeField] private float postDelay = 0.1f;

        [Header("효과")]
        public ParticleSystem skillVFX;
        private List<ParticleSystem> skillEffects = new();

        private List<HexTile> targetTiles = new();

        private Coroutine skillRoutine;

        private void Awake()
        {
            if(skillVFX != null)

            for (int i = 0; i < TargetCount; i++)
            {
                ParticleSystem vfx = Instantiate(skillVFX, Vector3.zero, Quaternion.identity, transform);
                skillEffects.Add(vfx);
            }
        }

        public override bool CanUse()
        {
            // 베이스 먼저 확인
            if(base.CanUse() == false) return false;
            if(owner.targetUnit == null) return false;  
            // 적이 멀리 있는 경우 사용 불가
            if(HexMath.Distance(owner.offset, owner.targetUnit .offset) > 1) return false;

            return true;
        }

        protected override void OnStart()
        {
            Vector3Int dir = HexMath.CubeDirections[0];     // (유닛이 없는 경우) 임의 방향으로 사용 -> TODO : 마지막 위치 등을 저장해서 사용 
            if(owner.targetUnit == null)
            {
                // 타겟이 없는 경우 재탐색 
                var target = UnitManager.Instance.GetNearestEnemy(owner);

                if (target == null)
                {
                    FinishSkill();      // 적을 찾지 못한경우 스킬 종료
                    return;
                }

                
                owner.SetTargetUnit(target);
                if(HexMath.Distance(owner.offset, target.offset) > 1)
                {
                    FinishSkill();      // 적이 사거리 내에 없으면 스킬 종료
                    return;
                }
            }
            // 타겟이 있을땐 해당 방향으로
            if(owner.targetUnit != null) dir = HexMath.GetDirectionCube(owner.offset, owner.targetUnit.offset);
            Vector3Int cube = HexMath.OffsetToCube(owner.offset);

            targetTiles.Clear();
            for (int i = 1; i <= TargetCount; i++)
            {
                targetTiles.Add(GridManager.Instance.GetTile(cube + dir * i));
            }

            skillRoutine = StartCoroutine(SkillCast());
        }

        IEnumerator SkillCast()
        {
            DamageInfo info = GetDamageInfo();

            // 선딜레이
            yield return new WaitForSeconds(preDelay);

            for (int i = 0; i < TargetCount; i++)
            {
                if (i >= targetTiles.Count || targetTiles[i] == null
                    || i >= skillEffects.Count || skillEffects[i] == null)
                    continue;

                // 타겟 타일에 유닛이 있는지 확인 & 적군인지 확인
                // TODO : 타일 기준으로 때릴지 확인
                UnitBase targetUnit = targetTiles[i].OccupantUnit;
                if (targetUnit != null && !targetUnit.IsSameTeam(owner.team))
                {
                    targetTiles[i].OccupantUnit.ApplyDamage(info);
                    // TODO : targetUnit.기절();
                }

                skillEffects[i].transform.position = targetTiles[i].transform.position + Vector3.up * 0.3f;
                skillEffects[i].Play();

                yield return null;
            }

            // 후딜레이
            yield return new WaitForSeconds(postDelay);

            FinishSkill();
        }

        DamageInfo GetDamageInfo()
        {
            // 베이스 데미지 설정
            float damage = baseDamage;

            // 계수 비례 데미지 계산
            damage += owner.statSet.AttackDamage.Value * ValueA;

            // TODO : 스킬의 치명타 여부 아이템 추가
            bool isCrit = false;
            // 치명타 확인
            //bool isCrit = DamageCalculator.RollCritical(owner.statSet.CritChance.Value);
            //if (isCrit)
            //{
            //    // 치명타 발생 시
            //    damage *= (owner.statSet.CritDamage.Value * 0.01f);
            //}

            // 피해 증가 배율 증가
            damage *= (100 + owner.statSet.DamageIncrease.Value) * 0.01f;


            DamageInfo info = new DamageInfo(owner, damage, DamageType.Physical, isCrit);
            owner.targetUnit?.ApplyDamage(info);
            // TODO : 스킬 시전 & 명중 이벤트 만들기

            return info;
        }

        protected override void OnCancel()
        {
            // 일반 공격은 강제 취소시에도 멈춤
            if (skillRoutine != null)
            {
                StopCoroutine(skillRoutine);
                skillRoutine = null;
            }
        }

        protected override void OnFinish()
        {
            // 코루틴 종료
            if (skillRoutine != null)
            {
                StopCoroutine(skillRoutine);
                skillRoutine = null;
            }
        }
    }
}
