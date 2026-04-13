using UnityEngine;
using Unit;
using StatSystem;
namespace Prototype.Skill
{ 
    public class MeleeUnit_NormalAttack : SkillBase
    {
        public ParticleSystem skillEffect;

        public override void UseSkill(UnitBase caster, UnitBase targetUnit)
        {
            Vector3 dir = caster.transform.position - targetUnit.transform.position;
            dir.y = 0f;
            transform.rotation = Quaternion.LookRotation(dir);

            skillEffect.Play();

            // 일반 공격은 공격력의 100* 데미지
            float damage = caster.statSet.AttackDamage.Value;           


            // 치명타 확인
            bool isCrit = DamageCalculator.RollCritical(caster.statSet.CritChance.Value);
            if(isCrit)
            {
                // 치명타 발생 시
                damage *= (caster.statSet.CritDamage.Value * 0.01f);
            }


            // 피해 증가 배율 증가
            damage *= (100 + caster.statSet.DamageIncrease.Value) * 0.01f;

            DamageInfo info = new DamageInfo(caster, damage, DamageType.Physical, isCrit);

            targetUnit?.ApplyDamage(info);
            caster.unitEvents.OnNormalAttackHit?.Invoke(info, targetUnit);
        }
    }
}
