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
    public class HexTile : DragButtonBase
    {
        [Header("컴포넌트 설정")]
        public TextMeshProUGUI unitName;
        public Image unitImage;

        [Header("성급 표시용 별")]
        public List<TileStar> stars = new();

        [Header("아이템 아이콘")]
        public List<Image> itemIcons = new();

        Vector2Int offset;

        [Header("데이터")]
        public UnitPlacement occupantUnitData;          // 점유 유닛의 모든 정보 (유닛, 성급, 아이템 정보 등)

        UnitDataSO curUnit => (IsEmpty ? null : occupantUnitData.unitData);

        /// <summary>
        /// 현재 해당 타일에 유닛이 있는지 여부를 반환합니다.
        /// </summary>
        public bool IsEmpty => occupantUnitData.unitData == null;


        public void Init(Vector2Int offset, Canvas canvas)
        {
            Init(canvas);
            this.offset = offset;

            ResetTile();
        }

        /// <summary>
        /// 유닛만 새롭게 추가
        /// </summary>
        /// <param name="unit"></param>
        public void SetNewUnit(UnitDataSO unit)
        {
            // 빈 타일에만 추가 가능
            if (!IsEmpty || unit == null) return;

            ResetTile();    // 초기화 한번 진행

            // 새로운 유닛 설정
            occupantUnitData.unitData = unit;
            occupantUnitData.star = 0;

            RefreshView();
        }

        /// <summary>
        /// 점유 유닛 정보 일괄 변경
        /// </summary>
        public void SetUnitData(UnitPlacement data) 
        {
            occupantUnitData = data;
            RefreshView();
        }

        /// <summary>
        /// 타일에 아이템 추가
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(ItemSO item)
        {
            if (IsEmpty) return;
            if (occupantUnitData.items.Count >= 3) return;

            occupantUnitData.items.Add(item);
            RefreshView();
        }


        // 유닛의 성급을 올린다.
        public void UpgradeStar()
        {
            if (IsEmpty) return;

            int maxStar = 3;

            // 유닛의 최대 성급에 맞춰서 별 개수 변경
            if (curUnit.statData != null)
                maxStar = curUnit.statData.statsByStart.Length;

            occupantUnitData.star = (occupantUnitData.star + 1) % maxStar;

            RefreshView();
        }

        // 유닛의 성급을 내린다.
        public void PreviousStar()
        {
            if (IsEmpty) return;

            int maxStar = 3;

            // 유닛의 최대 성급에 맞춰서 별 개수 변경
            if (curUnit.statData != null)
                maxStar = curUnit.statData.statsByStart.Length;

            occupantUnitData.star = (occupantUnitData.star + (maxStar - 1)) % maxStar;

            RefreshView();
        }

        /// <summary>
        /// 타일 정보 초기화
        /// </summary>
        public void ResetTile()
        {
            occupantUnitData.Clear(offset);
            RefreshView();
        }


        #region Refresh Views
        void RefreshView()
        {
            RefreshUnitView();
            RefreshStarView();
            RefreshItemView();
        }

        void RefreshUnitView()
        {
            if (IsEmpty)
            {
                unitName.enabled = false;
                unitImage.enabled = false;
                return;
            }

            // 이름 설정
            unitName.text = curUnit.Name_KR;
            unitName.enabled = true;

            // 아이콘 설정
            unitImage.sprite = curUnit.unitSprite;
            unitImage.enabled = true;
        }

        void RefreshStarView()
        {
            for (int i = 0; i < stars.Count; i++)
            {
                stars[i].gameObject.SetActive(i <= occupantUnitData.star);
            }
        }

        void RefreshItemView()
        {
            for(int i = 0; i < itemIcons.Count; i++)
            {
                if(i < occupantUnitData.items.Count && occupantUnitData.items[i] != null)
                {
                    itemIcons[i].gameObject.SetActive(true);
                    itemIcons[i].sprite = occupantUnitData.items[i].icon;
                }
                else
                {
                    itemIcons[i].gameObject.SetActive(false);
                }
            }
        }

        #endregion

        #region 마우스 이벤트

        bool CanDrag => !IsEmpty;

        public override void OnPointerClick(PointerEventData eventData)
        {
            // 우클릭 & 비어있지 않다면
            if (eventData.button == PointerEventData.InputButton.Right
                && !IsEmpty)
                ResetTile();
        }

        /// <summary>
        /// 타일을 다른 타일에 드래그 하였을때 호출
        /// </summary>
        protected override void Use(HexTile otherTile)
        {
            if (!CanDrag) return;

            if (otherTile.IsEmpty)        // 목표 타일이 비어있는 경우
            {
                // 데이터 복제, 전달 후 현재 타일 초기화
                otherTile.SetUnitData(new UnitPlacement(occupantUnitData));
                ResetTile();
            }
            else
            {
                // 목표 타일이 비어있지 않은 경우
                // 서로의 유닛 정보 교체
                UnitPlacement temp = new UnitPlacement(otherTile.occupantUnitData);
                otherTile.SetUnitData(occupantUnitData);
                SetUnitData(temp);
            }
        }


        // 드래그 관련 메서드는 타일이 비어있지 않을때만 사용 가능하도록 설정
        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (!CanDrag) return;
            base.OnBeginDrag(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (!CanDrag) return;
            base.OnDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if(IsEmpty) return;
            base.OnEndDrag(eventData);
        }
        #endregion
    }
}
