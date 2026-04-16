using Unit;
using UnityEngine;

public struct HitInfo
{
    public float amount;          // 실제 적용된 값
    public float originalAmount;  // 원본

    public HitType hitType;     // Damage / Heal / Shield

    public UnitBase caster;
    public UnitBase target;

    public bool isCritical;

    public HitInfo(
           float amount,
           float originalAmount,
           HitType hitType,
           UnitBase caster,
           UnitBase target,
           bool isCritical
       )
    {
        this.amount = amount;
        this.originalAmount = originalAmount;
        this.hitType = hitType;
        this.caster = caster;
        this.target = target;
        this.isCritical = isCritical;
    }
}


