using UnityEngine;
using UnityEngine.EventSystems;
using Prototype.Grid;
using Unit;
using System.Collections.Generic;
using Game.UI;

namespace Unit
{

    public class UnitDragHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [Header("Drag Settings")]
        public float hoverHeight = 0.5f;
        public LayerMask tileLayer;

        [Header("UnitBase")]
        [SerializeField] private UnitBase _unit;

        Vector3 originPos;
        HexTile originTile;
        bool canDrag = false;

        private void Start() 
        {
            if(_unit != null)
                canDrag = _unit.IsSameTeam(TeamType.Ally);
        }

        // 마우스 클릭
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            // Debug.Log("클릭!");
            if (_unit == null) return;


            if(DetailPanelController.Instance != null)
            {
                DetailPanelController.Instance.ShowUnitDetail(_unit);
            }
        }

        #region Pointer Event
        public void OnPointerEnter(PointerEventData eventData)
        {
            // Debug.Log("마우스 올라옴");
        }

        // 마우스가 카드를 벗어남
        public void OnPointerExit(PointerEventData eventData)
        {
            // Debug.Log("마우스 내려감");
        }

        #endregion

        #region Drag Event

        // 드래그 시작
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;      // 좌클릭만 드래그 가능

            if (_unit == null || !canDrag) return;

            Debug.Log("드래그 시작");

            //Debug.Log("드래그 시작");
            originPos = _unit.transform.position;
            originTile = _unit.currentTile;

            // 프리뷰 활성화
            GridManager.Instance.OnBeginDrag?.Invoke(TeamType.Ally);
        }

        // 드래그 중
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;      // 좌클릭만 드래그 가능

            if (_unit == null || !canDrag
                || originTile == null) return;

             Ray ray = Camera.main.ScreenPointToRay(eventData.position);

            Plane plane = new Plane(Vector3.up, new Vector3(0, originPos.y + hoverHeight, 0)); // 기존 높이에서 0.5 띄움

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 worldPos = ray.GetPoint(distance);
                _unit.transform.position = worldPos;
            }
        }

        // 드래그 종료
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;      // 좌클릭만 드래그 가능

            if (_unit == null || !canDrag
                || originTile == null) return;

            if(!TryMoveToTile(eventData))        // 이동에 실패한 경우 돌아가기
                _unit.transform.position = originPos;
            
            GridManager.Instance.OnEndDrag?.Invoke();
        }

        public bool TryMoveToTile(PointerEventData eventData)
        {      
            Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, tileLayer))
            {
                if (hit.collider.TryGetComponent(out HexTile tile))
                {
                    if(tile.TeamType != TeamType.Ally) return false;

                    if(tile.OccupantUnit == null)
                    {
                        _unit.SetTile(tile);
                        _unit.transform.position = tile.transform.position;
                    }
                    else if(tile.OccupantUnit != _unit)
                    {
                        // 타겟 타일에 유닛이 있는 경우 (자기 자신이 아니면)
                        originTile.ExitTile(_unit);

                        // 타겟 유닛 먼저 이동
                        UnitBase targetUnit = tile.OccupantUnit;
                        targetUnit.SetTile(originTile);
                        targetUnit.transform.position = originTile.transform.position;

                        _unit.SetTile(tile);
                        _unit.transform.position = tile.transform.position;
                    }

                    return true;
                }
            }
           

            Debug.Log("타일 위 아님");
            return false;
        }
        #endregion
    }
}
