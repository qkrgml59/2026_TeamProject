using UnityEngine;
using Unit;

public struct DamageInfo
{
    public UnitBase caster { get; private set; }
    public float amount { get; private set; }
    public DamageType damageType { get; private set; }
    public bool isCritical { get; private set; }

    public DamageInfo(UnitBase caster, float amount, DamageType damageType, bool isCritical)
    {
        this.caster = caster;
        this.amount = amount;
        this.damageType = damageType;
        this.isCritical = isCritical;
    }
}
