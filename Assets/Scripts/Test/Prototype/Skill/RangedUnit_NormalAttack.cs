using Prototype.Unit;
using UnityEngine;

namespace Prototype.Skill
{
    public class RangedUnit_NormalAttack : SkillBase
    {
        public RangedUnit_TestProjectile projectilePrefab;
        public override void UseSkill(UnitBase caster, UnitBase targetUnit)
        {
            Vector3 dir = caster.transform.position - targetUnit.transform.position;
            dir.y = 0f;
            RangedUnit_TestProjectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.LookRotation(dir));
            projectile.Initialize(caster, targetUnit);
        }
    }
}
