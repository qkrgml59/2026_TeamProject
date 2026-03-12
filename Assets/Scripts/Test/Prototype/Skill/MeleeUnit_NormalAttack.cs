using UnityEngine;
using Prototype.Unit;
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
            targetUnit?.ApplyDamage(caster.statSet.AttackDamage.Value);
        }
    }
}
