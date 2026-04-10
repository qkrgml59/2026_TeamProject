using UnityEngine;
using Unit;

namespace Item
{
    public abstract class ItemEffectSO : ScriptableObject
    {
        // 아이템이 유닛에게 장착될 때 한 번 호출
        public abstract void OnEquip(UnitBase unit);

        // 아이템이 유닛에게서 해제될 때 호출
        public abstract void OnUnequip(UnitBase unit);
    }
}
