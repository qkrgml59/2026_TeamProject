using UnityEngine;
using Prototype.Unit;

namespace Prototype.Skill
{
    // 일단 기본공격도 스킬로 구현
    public abstract class SkillBase : MonoBehaviour
    {
        // TODO: 코스트나 스킬 설명은 추가
        public abstract void UseSkill(UnitBase caster, UnitBase targetUnit);
    }
}
