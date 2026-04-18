using Item;
using System.Collections.Generic;
using Unit;
using UnityEngine;

namespace Game.UI
{
    public class UnitItemView : MonoBehaviour
    {
        public ItemSlotView[] slots;

        public void Clear()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].Hide();
            }
        }

        public void UpdateItems(List<ItemBase> items)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (i < items.Count)
                    slots[i].Show(items[i]);
                else
                    slots[i].Hide();
            }
        }
    }
}
