using Item;
using System.Collections.Generic;
using TMPro;
using Unit;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameEditor.UnitPlacer
{
    public class HexTile : PlacerButton
    {
        [Header("컴포넌트 설정")]
        public TextMeshProUGUI unitName;
        public Image unitImage;
        public List<Image> stars = new();
        public List<Image> itemIcons = new();

        [Header("타일 정보")]
        public Vector2Int offset;

        [Header("데이터")]
        public StageUnitData occupantUnit;

        public bool IsEmpty => occupantUnit.unitData == null;

        private void Awake()
        {
            ResetTile();
        }

        public void SetNewUnit(UnitDataSO unit)
        {
            if (!IsEmpty || unit == null) return;
            ResetTile();
            occupantUnit.unitData = unit;
            occupantUnit.star = 0;

            SetUnitView();
            SetItemImage();
            SetStarImage();
        }

        void SetUnitView()
        {
            if(IsEmpty)
            {
                ResetTile();
                return;
            }

            unitName.text = occupantUnit.unitData.Name_KR;
            unitName.enabled = true;

            unitImage.sprite = occupantUnit.unitData.unitSprite;
            unitImage.enabled = true;
        }

        public void SetItem(ItemSO item)
        {
            if(occupantUnit.items.Count >= 3) return;

            occupantUnit.items.Add(item);
            SetItemImage();
        }

        /// <summary>
        /// 유닛 데이터 통째로 변경
        /// </summary>
        public void SetUnitData(StageUnitData data) 
        {
            occupantUnit = data;
            SetUnitView();
            SetItemImage();
            SetStarImage();
        }

        public void ResetTile()
        {
            occupantUnit.Clear();
            SetStarImage();
            SetItemImage();

            unitName.enabled = false;
            unitImage.enabled = false;
        }
        
        void SetStarImage()
        {
            for(int i = 0; i < stars.Count; i++)
            {
                stars[i].gameObject.SetActive(i <= occupantUnit.star);
            }
        }

        void SetItemImage()
        {
            for(int i = 0; i < itemIcons.Count; i++)
            {
                if(i < occupantUnit.items.Count)
                {
                    itemIcons[i].gameObject.SetActive(true);
                    itemIcons[i].sprite = occupantUnit.items[i].icon;
                }
                else
                {
                    itemIcons[i].gameObject.SetActive(false);
                }
            }
        }


        // 마우스 이벤트

        public override void OnPointerClick(PointerEventData eventData)
        {
            // 우클릭이면 유닛 제거
            if (eventData.button == PointerEventData.InputButton.Right)
                ResetTile();
        }

        protected override void Use(HexTile tile)
        {
            if (IsEmpty) return;

            if(tile.IsEmpty)        // 목표 타일이 비어있는 경우
            {
                // 데이터 넣고, 현재 타일 비우기
                tile.SetUnitData(occupantUnit.Clone());
                ResetTile();
            }
            else
            {
                StageUnitData temp = tile.occupantUnit.Clone();
                tile.SetUnitData(occupantUnit);
                SetUnitData(temp);
            }
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (IsEmpty) return;
            base.OnBeginDrag(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (IsEmpty) return;
            base.OnDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if(IsEmpty) return;
            base.OnEndDrag(eventData);
        }   
    }
}
