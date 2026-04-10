using Prototype.Grid;
using Stat;
using System.Collections.Generic;
using Unit;
using UnityEngine;

namespace Item
{
    public class ItemBase : MonoBehaviour
    {
        
        [Header("Item Data")] public ItemSO itemData;

        private List<ItemEffectSO> _activeEffects = new List<ItemEffectSO>();

        #region 장착 해제 관련

        public bool TryEquip(UnitBase unit)
        {

            // 아이템 장착 처리.
            unit.EquippedItems.Add(this);       // 유닛 아이템 장착 리스트에 추가

            transform.SetParent(unit.transform);        // 아이템을 유닛의 자식으로 지정

            //TODO : 시각적 위치 조정
            // 예시 : transform.localPosition = new Vector3(0, 1.5f, 0);

            ApplyItemEffect(unit);              // 아이템 효과 스탯 or 특수 효과(조합 아이템) 적용

            Debug.Log($"[{unit.name}]에게 [{itemData.itemName}] 장착.");
            return true;
        }

        public void UnequipFrom(UnitBase unit)
        {
            if (unit.EquippedItems.Contains(this))
            {
                RemoveItemEffect(unit);     // 아이템 효과 해제
                unit.EquippedItems.Remove(this);        // 유닛 아이템 장착 리스트 제거


                transform.SetParent(null);      // 유닛 자식 해제

                // TODO: 카드로 이동
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

                    ItemEffectSO effectInstance = Instantiate(effectData);
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
                Destroy(effectInstance);
            }
            _activeEffects.Clear();
        }

        #endregion
    }
}
