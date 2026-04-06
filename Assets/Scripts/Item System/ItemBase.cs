using Prototype.Grid;
using Unit;
using UnityEngine;

namespace Item
{
    public abstract class ItemBase : MonoBehaviour
    {
        
        [Header("Item Data")] public ItemSO itemData;
    

        #region 장착 해제 관련

        public bool TryEquip(UnitBase unit)
        {
            if (unit.team != TeamType.Ally)     // 아군일 때만 장착 가능.
            {
                Debug.LogWarning("아군에게만 아이템을 장착할 수 있습니다.");
                return false;
            }

            // TODO : 장착 된 아이템 체크 후 조합 가능한지 체크

            if (unit.EquippedItems.Count >= 3)          // 장착 중인 아이템 개수가 3개 보다 클 때
            {
                Debug.LogWarning("아이템은 3개까지만 장착 가능합니다.");
                 return false;
            }

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

        protected abstract void ApplyItemEffect(UnitBase unit);                 // 아이템 장착 효과 (장착 시)
        protected abstract void RemoveItemEffect(UnitBase unit);                // 아이템 해제 효과 (해제 시)

        #endregion
    }
}
