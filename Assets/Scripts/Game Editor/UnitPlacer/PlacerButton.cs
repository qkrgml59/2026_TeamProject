using Prototype.Card;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameEditor.UnitPlacer
{
    public abstract class PlacerButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public Canvas canvas;
        public RectTransform childRect;

        public LayerMask tileLayer;
        protected Vector2 _originAnchoredPos;

        public void Init(Canvas canvas)
        {
            this.canvas = canvas;
        }

        #region Hover Event
        // 마우스가 카드 위에 올라옴
        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("마우스 올라옴");
        }

        // 마우스가 카드를 벗어남
        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("마우스 내려감");
        }

        #endregion

        // 마우스 클릭
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("클릭!");
        }

        #region Drag Event

        // 드래그 시작
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("드래그 시작");
            childRect.SetAsLastSibling();
            _originAnchoredPos = childRect.anchoredPosition;
            childRect.SetParent(canvas.transform);
        }

        // 드래그 중
        public virtual void OnDrag(PointerEventData eventData)
        {
            Vector3 mousePos = eventData.position;
            childRect.position = mousePos;
        }

        // 드래그 종료
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("드래그 종료");

            CheckTileUnderMouse();

            // 무조건 돌아가기
            childRect.SetParent(transform);
            childRect.anchoredPosition = _originAnchoredPos;
        }

        public void CheckTileUnderMouse()
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var result in results)
            {
                HexTile tile = result.gameObject.GetComponent<HexTile>();

                if (tile != null)
                {
                    Debug.Log("타일 위에서 드래그 종료");
                    Use(tile);
                    return;
                }
            }

            Debug.Log("타일 위 아님");
        }

        protected abstract void Use(HexTile tile);
        #endregion
    }

}
