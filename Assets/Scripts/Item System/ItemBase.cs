using Prototype.Grid;
using Unit;
using UnityEngine;

namespace Item
{
    public abstract class ItemBase : MonoBehaviour
    {
        
        [Header("Item Data")] public ItemSO itemData;

        public float dragHeight = 1.0f;             // groundPlane에서 높여놓을 높이;

        private Vector3 originPos;                  // 아이템 사용 실패(유닛이 아닌 이상한 곳에서 놓았을 때)시 돌아갈 위치
        private Plane groundPlane;                  // 드래그 할 때 사용할 플레인


        #region 마우스 드래그 앤 드롭

        private void OnMouseDown()
        {
            originPos = transform.position;
            groundPlane = new Plane(Vector3.up, new Vector3(0, dragHeight, 0));
        }

        private void OnMouseDrag()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (groundPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                transform.position = hitPoint;
            }
        }

        private void OnMouseUp()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // RayCast 맞은게 유닛인지 검사
                UnitBase target = hit.collider.GetComponent<UnitBase>();

                if (target == null)     // 만약 타겟 유닛이 아닌 타일에 놓았을 때
                {
                    HexTile tile = hit.collider.GetComponent<HexTile>();

                    if (tile != null)
                    {
                        target = tile.OccupantUnit;
                    }
                }

                if (target != null && TryEquip(target))
                {
                    return;
                }
            }

            ReturnOriginalPos();    // 장착 실패 시 원래(드래그 전) 자리로.
        }

        void ReturnOriginalPos()
        {
            transform.position = originPos;
        }

        #endregion

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
            unit.EquippedItems.Add(this);
            ApplyItemEffect(unit);

            gameObject.SetActive(false);
            // TODO : UI.

            Debug.Log($"[{unit.name}]에게 [{itemData.itemName}] 장착.");
            return true;
        }

        public void UnequipFrom(UnitBase unit)
        {
            if (unit.EquippedItems.Contains(this))
            {
                unit.EquippedItems.Remove(this);
                RemoveItemEffect(unit);

                // 다시 필드에 보이게 처리
                gameObject.SetActive(true);
                transform.SetParent(null);

                // TODO: 대기석이나 아이템 존으로 위치 이동
            }
        }

        protected abstract void ApplyItemEffect(UnitBase unit);                 // 아이템 장착 효과 (장착 시)
        protected abstract void RemoveItemEffect(UnitBase unit);                // 아이템 해제 효과 (해제 시)

        #endregion
    }
}
