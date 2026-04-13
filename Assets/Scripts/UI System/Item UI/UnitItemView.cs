using Item;
using System.Collections.Generic;
using Unit;
using UnityEngine;

namespace Game.UI
{
    public class UnitItemView : MonoBehaviour
    {
        public ItemSlotView[] slots;
        public RectTransform rectTransform;

        [Header("위치 설정")]
        public float yOffset = 3.5f;

        UnitBase _Unit;

        public void Init(UnitBase unit)
        {
            _Unit = unit;

            UpdatePosition();
            UpdateItems(_Unit.EquippedItems);
            _Unit.unitEvents.OnItemChanged.AddListener(UpdateItems);
        }

        private void Update()
        {
            if (_Unit != null && _Unit.CurFSM != UnitStateType.Dead)
                UpdatePosition();
        }

        public void UpdatePosition()
        {
            Vector3 worldPos = _Unit.transform.position;
            worldPos += Vector3.up * yOffset;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            rectTransform.position = screenPos;
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

        private void OnDisable()
        {
            if (_Unit != null && _Unit.unitEvents != null)
                _Unit.unitEvents.OnItemChanged.RemoveListener(UpdateItems);
        }
    }
}
