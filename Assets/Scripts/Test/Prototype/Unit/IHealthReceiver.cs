using UnityEngine;

namespace Prototype.Unit
{
    public interface IHealthReceiver
    {
        void ApplyDamage(float amount);
        void ApplyHeal(float amount);
    }
}
