using Item;
using System.Collections.Generic;
using Unit;
using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// 유닛 상태 바 View 들을 모아둔 묶음입니다.
    /// </summary>
    public class UnitStatusView : MonoBehaviour
    {
        [Header("참조용 View")]
        public UnitHpView hpView;
        public UnitItemView itemView;
        public UnitResourceView resourceView;

        // 유닛 추적용
        RectTransform rectTransform;
        UnitBase target;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void SetTarget(UnitBase unit)
        {
            if (unit == null) return;
            target = unit;

            // 포지션 초기화
            UpdatePosition();
        }

        void Update()
        {
            if (target != null && target.CurFSM != UnitStateType.Dead)
                UpdatePosition();
        }

        void UpdatePosition()
        {
            if (rectTransform == null) return;

            Vector3 worldPos = target.transform.position;
            worldPos += Vector3.up * 4f;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            rectTransform.position = screenPos;
        }

        public void OnItemChanged(List<ItemBase> items)
        {
            if (itemView == null) return;

            itemView.UpdateItems(items);
            itemView.gameObject.SetActive(items.Count > 0);
        }
    }
}
