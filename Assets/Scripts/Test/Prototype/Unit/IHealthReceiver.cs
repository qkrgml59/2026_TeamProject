using UnityEngine;

namespace Unit
{
    public interface IHealthReceiver
    {
        void ApplyDamage(DamageInfo info);
        void ApplyHeal(HealInfo info);
    }
}
