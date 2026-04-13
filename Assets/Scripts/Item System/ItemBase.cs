using Prototype.Grid;
using Stat;
using System.Collections.Generic;
using Unit;
using UnityEngine;

namespace Item
{
    public class ItemBase
    {
        
        [Header("Item Data")] public ItemSO itemData;

        private List<ItemEffectSO> _activeEffects = new List<ItemEffectSO>();

        public ItemBase(ItemSO data)
        {
            this.itemData = data;
        }

        #region 장착 해제 관련

        public bool TryEquip(UnitBase unit)
        {
            // 아이템 장착 처리.
            if (unit.TryEquippedItem(this))
            {
                ApplyItemEffect(unit);
                Debug.Log($"[{unit.name}]에게 [{itemData.itemName}] 장착.");
                return true;
            }
            return false;
        }

        public void UnequipItem(UnitBase unit)
        {
            if (unit.EquippedItems.Contains(this))
            {
                RemoveItemEffect(unit);

                unit.UnequipItem(this);

                // TODO: 카드로 이동하거나 바닥에 떨어뜨리는 처리
            }
        }

        protected virtual void ApplyItemEffect(UnitBase unit)                 // 아이템 장착 효과 (장착 시)
        {
            if (itemData == null) return;

            // 스탯 적용
            if (itemData.modifiers != null)
            {
                foreach (StatModifier modifier in itemData.modifiers)
                {
                    unit.AddStatModifier(modifier.statType, this, modifier);
                }
            }

            // 특수 효과 모듈 장착
            if (itemData.effectModules != null)
            {
                foreach (ItemEffectSO effectData in itemData.effectModules)
                {
                    if (effectData == null)
                    {
                        Debug.LogWarning($"[{itemData.itemName}]의 특수효과 모듈에 빈칸이 있습니다!");
                        continue;
                    }

                    ItemEffectSO effectInstance = Object.Instantiate(effectData);
                    effectInstance.OnEquip(unit);
                    _activeEffects.Add(effectInstance); 
                }
            }
        }

        protected virtual void RemoveItemEffect(UnitBase unit)                // 아이템 해제 효과 (해제 시)
        {
            if (itemData == null) return;

            unit.RemoveStatModifier(this);

            // 장착했던 특수 효과 모듈들 해제 
            foreach (ItemEffectSO effectInstance in _activeEffects)
            {
                effectInstance.OnUnequip(unit);
                Object.Destroy(effectInstance);
            }
            _activeEffects.Clear();
        }

        #endregion
    }
}
