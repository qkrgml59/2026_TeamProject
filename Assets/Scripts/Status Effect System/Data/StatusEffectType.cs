using UnityEngine;

namespace Unit.StatusEffect
{

    public enum StatusEffectType
    {
        Null = 0,               // 없음
        Stun,                   // 기절
        DefenseDown,            // 방어력 감소
        MagicResistanceDown,    // 마법 저항력 감소
        HealingReduction,       // 치유 감소
        Burn,                   // 화상
    }

}