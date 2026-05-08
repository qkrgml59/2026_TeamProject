using UnityEngine;

// 모든 상태이상의 데이터를 관리하는 클래스 입니다.

namespace Unit.StatusEffect
{
    public abstract class StatusEffect
    {
        public abstract StatusEffectType EffectType { get; }
        public abstract string EffectName { get; }
        public abstract string EffectDiscription { get; }
    }


    /// <summary>
    /// 기절 상태이상
    /// </summary>
    public class Stun : StatusEffect
    {
        public override StatusEffectType EffectType => StatusEffectType.Stun;
        public override string EffectName => "기절";
        public override string EffectDiscription => $"<color=#575757>{EffectName}- 이동, 기본 공격, 스킬 사용이 불가능합니다.</color>";

    }


    /// <summary>
    /// 방어력 감소 상태이상
    /// </summary>
    public class DefenseDown : StatusEffect
    {
        public override StatusEffectType EffectType => StatusEffectType.DefenseDown;
        public override string EffectName => "파열";
        public override string EffectDiscription => "<color=#575757>기절 - 이동, 기본 공격, 스킬 사용이 불가능합니다.</color>";
    }


    /// <summary>
    /// 마법 저항력 감소 상태이상
    /// </summary>
    public class MagicResistanceDown : StatusEffect
    {
        public override StatusEffectType EffectType => StatusEffectType.MagicResistanceDown;
        public override string EffectName => "파쇄";
        public override string EffectDiscription => "<color=#575757>기절 - 이동, 기본 공격, 스킬 사용이 불가능합니다.</color>";
    }


    /// <summary>
    /// 치유 감소
    /// </summary>
    public class HealingReduction : StatusEffect
    {
        public override StatusEffectType EffectType => StatusEffectType.HealingReduction;
        public override string EffectName => "기절";
        public override string EffectDiscription => "<color=#575757>기절 - 이동, 기본 공격, 스킬 사용이 불가능합니다.</color>";
    }


    /// <summary>
    /// 화상
    /// </summary>
    public class Burn : StatusEffect
    {
        public override StatusEffectType EffectType => StatusEffectType.Burn;
        public override string EffectName => "기절";
        public override string EffectDiscription => "<color=#575757>기절 - 이동, 기본 공격, 스킬 사용이 불가능합니다.</color>";
    }
}