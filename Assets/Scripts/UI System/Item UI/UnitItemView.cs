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
            List<Sprite> icons = new List<Sprite>();
            foreach (var item in items)
            {
                if (item.itemData != null && item.itemData.icon != null)
                    icons.Add(item.itemData.icon);
            }

            for (int i = 0; i < slots.Length; i++)
            {
                if (i < icons.Count)
                    slots[i].Show(icons[i]);
                else
                    slots[i].Hide();
            }
        }
    }
}
