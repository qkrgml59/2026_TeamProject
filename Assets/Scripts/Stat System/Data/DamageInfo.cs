using UnityEngine;
using Unit;

public struct DamageInfo
{
    public UnitBase source { get; private set; }
    public float amount { get; private set; }
    public DamageType damageType { get; private set; }
    public bool isCritical { get; private set; }

    public DamageInfo(UnitBase source, float amount, DamageType damageType, bool isCritical)
    {
        this.source = source;
        this.amount = amount;
        this.damageType = damageType;
        this.isCritical = isCritical;
    }
}
