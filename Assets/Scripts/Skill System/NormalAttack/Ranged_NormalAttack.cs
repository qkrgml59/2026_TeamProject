using Prototype.Skill;
using StatSystem;
using System.Collections;
using UnityEngine;

namespace Unit.Skill
{
    public class Ranged_NormalAttack : SkillBase
    {
        [Header("투사체")] public Ranged_NormalAttack_Projectile projectilePrefab;
        [Header("공격 선딜레이 비율 (%)")] public float windupRatio = 0.3f;

        private float attackCycle;

        Coroutine attackRoutine;


        protected override void OnStart()
        {
            attackCycle = 1f / owner.statSet.AttackSpeed.Value;

            if (attackRoutine != null)
            {
                StopCoroutine(attackRoutine);
            }

            attackRoutine = StartCoroutine(AttackRoutine());
        }
        IEnumerator AttackRoutine()
        {
            //선딜레이
            yield return new WaitForSeconds(attackCycle * windupRatio);

            // 투사체 발사
            FireProjectile();
            isUsing = false;        // 데미지가 들어가면 재사용 가능하도록

            // 후딜레이
            yield return new WaitForSeconds(attackCycle * (1 - windupRatio));

            FinishSkill();
        }

        void FireProjectile()
        {
            // 일반 공격은 공격력의 100% 데미지
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

            Ranged_NormalAttack_Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.Initialize(info, owner.targetUnit);
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
