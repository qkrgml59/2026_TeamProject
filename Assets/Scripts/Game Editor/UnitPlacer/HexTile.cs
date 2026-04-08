using Item;
using System.Collections.Generic;
using TMPro;
using Unit;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GameEditor.UnitPlacer
{
    public class HexTile : MonoBehaviour
    {
        [Header("컴포넌트 설정")]
        public TextMeshProUGUI unitName;
        public Image unitImage;
        public List<Image> stars = new();
        public List<Image> itemIcons = new();

        [Header("타일 정보")]
        public Vector2Int offset;

        [Header("데이터")]
        public StageUnitData unitData;

        private void Awake()
        {
            ResetTile();
        }

        public void SetUnit(UnitDataSO unit)
        {
            if (unitData.unitData != null || unit == null) return;
            unitData.unitData = unit;

            unitName.text = unit.Name_KR;
            unitName.enabled = true;

            unitImage.sprite = unit.unitSprite;
            unitImage.enabled = true;

            SetStarImage(0);
        }

        public void RemoveUnit()
        {
            if (unitData.unitData == null) return;
            ResetTile();
        }

        public void SetItem(ItemSO item)
        {
            if(unitData.items.Count >= 3) return;

            unitData.items.Add(item);
            SetItemImage();
        }

        public void SetUnitData(StageUnitData data) 
        {
            unitData = data;
            unitData.offset = offset;
            SetUnit(unitData.unitData);
            SetItemImage();
            SetStarImage(unitData.star);
        }

        public void ResetTile()
        {
            unitData.Clear();
            SetStarImage(-1);
            SetItemImage();

            unitName.enabled = false;
            unitImage.enabled = false;
        }
        
        void SetStarImage(int star)
        {
            for(int i = 0; i < stars.Count; i++)
            {
                stars[i].gameObject.SetActive(i <= star);
            }
        }

        void SetItemImage()
        {
            for(int i = 0; i < itemIcons.Count; i++)
            {
                if(i < unitData.items.Count)
                {
                    itemIcons[i].gameObject.SetActive(true);
                    itemIcons[i].sprite = unitData.items[i].icon;
                }
                else
                {
                    itemIcons[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
