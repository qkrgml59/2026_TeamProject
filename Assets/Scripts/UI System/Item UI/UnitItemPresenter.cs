using System.Collections.Generic;
using UnityEngine;
using Unit;
using Item;

namespace Game.UI
{
    public class UnitItemPresenter : MonoBehaviour
    {
        public UnitItemView ItemSlotViewPrefab;
        public Canvas ItemSlotCanvas; 

        Dictionary<UnitBase, UnitItemView> viewData = new();

        public void RegisterItemSlot(UnitBase unit)
        {
            if (viewData.ContainsKey(unit))
            {
                //viewData[unit].Init(unit);
            }
            else
            {
                if (ItemSlotViewPrefab == null)
                {
                    Debug.LogWarning("ItemSlotViewPrefab이 없음.", this);
                    return;
                }

                if (ItemSlotCanvas == null)
                {
                    Debug.LogWarning("ItemSlotCanvas가 없습니다.", this);
                    return;
                }

                UnitItemView view = Instantiate(ItemSlotViewPrefab, ItemSlotCanvas.transform);
                //view.Init(unit);
                viewData.Add(unit, view);
            }

            unit.unitEvents.OnDestroyedUnit.AddListener(RemoveItemSlot);
        }

        public void RemoveItemSlot(UnitBase unit)
        {
            unit.unitEvents.OnDestroyedUnit.RemoveListener(RemoveItemSlot);

            if (viewData.ContainsKey(unit))
            {
                UnitItemView view = viewData[unit];
                Destroy(view.gameObject);
                viewData.Remove(unit);
            }
        }

        private void OnDestroy()
        {
            foreach (var unit in viewData.Keys)
            {
                if (unit != null && unit.unitEvents != null)
                    unit.unitEvents.OnDestroyedUnit.RemoveListener(RemoveItemSlot);
            }
        }
    }
}
