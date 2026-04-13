using System.Collections;
using UnityEngine;
using StatSystem;

namespace Unit.Skill
{
    public class Melee_NormalAttack : SkillBase
    {
        [Header("공격 이펙트")] public ParticleSystem skillEffect;
        private ParticleSystem[] particles;     // 효과 적용을 위한 모든 파티클 저장


        [Header("공격 선딜레이 비율 (%)")] public float windupRatio = 0.3f;

        private float attackCycle;

        Coroutine attackRoutine;

        private void Awake()
        {
            particles = skillEffect.GetComponentsInChildren<ParticleSystem>(true);
        }

        protected override void OnStart()
        {
            float attackSpeed = owner.statSet.AttackSpeed.Value;
            attackCycle = 1f / attackSpeed;
            SetEffectSpeed(attackSpeed);

            if(attackRoutine != null)
            {
                StopCoroutine(attackRoutine);
            }

            attackRoutine = StartCoroutine(AttackRoutine());
        }

        // 파티클 속도 조절
        public void SetEffectSpeed(float speed)
        {
            foreach (var ps in particles)
            {
                var main = ps.main;
                main.simulationSpeed = speed;
            }
        }

        IEnumerator AttackRoutine()
        {
            // 방향 설정
            Vector3 dir = owner.transform.position - owner.targetUnit.transform.position;
            dir.y = 0f;
            transform.rotation = Quaternion.LookRotation(dir);

            skillEffect.Play();

            //선딜레이
            yield return new WaitForSeconds(attackCycle * windupRatio);

            TakeDamage();
            isUsing = false;        // 데미지가 들어가면 재사용 가능하도록

            // 후딜레이
            yield return new WaitForSeconds(attackCycle * (1 - windupRatio));

            FinishSkill();
        }

        void TakeDamage()
        {
            // 일반 공격은 공격력의 100* 데미지
            float damage = owner.statSet.AttackDamage.Value;

            // 치명타 확인
            bool isCrit = DamageCalculator.RollCritical(owner.statSet.CritChance.Value);
            if (isCrit)
            {
                // 치명타 발생 시
                damage *= (owner.statSet.CritDamage.Value * 0.01f);
            }

            // 피해 증가 배율 증가
            damage *= (100 + owner.statSet.DamageIncrease.Value) * 0.01f;


            DamageInfo info = new DamageInfo(owner, damage, DamageType.Physical, isCrit);
            owner.targetUnit?.ApplyDamage(info);
            owner.unitEvents.OnNormalAttackHit?.Invoke(info, owner.targetUnit);
        }

        protected override void OnCancel()
        {
            // 일반 공격은 강제 취소시에도 멈춤
            if (attackRoutine != null)
            {
                StopCoroutine(attackRoutine);
                attackRoutine = null;
            }
        }

        protected override void OnFinish()
        {
            // 코루틴 종료
            if (attackRoutine != null)
            {
                StopCoroutine(attackRoutine);
                attackRoutine = null;
            }
        }
    }
}
