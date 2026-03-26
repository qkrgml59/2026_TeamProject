using UnityEngine;
using Unit;

public struct HealInfo
{
    public UnitBase source { get; private set; }
    public float amount { get; private set; }

    public HealInfo(UnitBase source, float amount)
    {
        this.source = source;
        this.amount = amount;
    }
}
