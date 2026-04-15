using Prototype.Grid;
using Prototype.UI;
using Unit;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Prototype.Card
{
    public abstract class CardBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public CardDataSO cardData;
        public Image icon;
        public TextMeshProUGUI cardName;
        public TextMeshProUGUI cardDescription;
        public RectTransform rectTransform;

        public LayerMask tileLayer;
        protected Vector2 _originAnchoredPos;

        protected HexTile currentTile = null;

        protected virtual void Awake()
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
        }

        public virtual void Init(CardDataSO data)
        {
            cardData = data;
            icon.sprite = data.icon;
            cardName.text = data.cardName;
            cardDescription.text = data.description;
        }

        #region Preview Setting

        public abstract TeamType GetTargetTile();

        #endregion

        #region Hover Event
        // 마우스가 카드 위에 올라옴
        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("마우스 올라옴");
            rectTransform.localScale = Vector2.one * 1.1f;
        }

        // 마우스가 카드를 벗어남
        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("마우스 내려감");
            rectTransform.localScale = Vector2.one;
        }

        #endregion

        // 마우스 클릭
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("클릭!");
        }

        #region Drag Event

        // 드래그 시작
        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("드래그 시작");
            rectTransform.SetAsLastSibling();
            _originAnchoredPos = rectTransform.anchoredPosition;

            // 프리뷰 활성화
            GridManager.Instance.OnBeginDrag?.Invoke(GetTargetTile());
        }

        // 드래그 중
        public void OnDrag(PointerEventData eventData)
        {
            Vector3 mousePos = eventData.position;
            rectTransform.position = mousePos;
        }

        // 드래그 종료
        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("드래그 종료");

            Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            bool isUsed = false;

            if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, tileLayer))
            {
                if (CostManager.Instance.CheckCost(cardData.cardCost))
                {
                    Debug.Log($"카드 소환 성공! 남은 코스트 : {CostManager.Instance.currentCost} / {CostManager.Instance.maxCost}");
                    isUsed = TryUseCard(hit);
                }
                else
                {
                    Debug.LogWarning($"코스트 부족");
                    isUsed = false;
                }

                // 변경 전 코드
                /*
                HexTile tile = hit.transform.GetComponent<HexTile>();
                if (tile != null)
                {

                      변경 전 코드
                     일단 중복 설치 방지
                    if (tile.CanReserve(null))
                    {
                        UnitBase unit = Instantiate(cardData.unit, tile.transform.position, Quaternion.identity);
                        unit.team = TeamType.Ally;
                        tile.EnterTile(unit);
                        unit.EnterTile(tile);
                        UnitManager.Instance.RegisterUnit(unit);
                        IndicatorManager.Instance.HPBarPresenter.RegisterHealthBar(unit);

                        CardManager.Instance.UseCard(this);
                        return;
                    }
                }*/
            }

            if (isUsed)     //카드 사용 성공 시
            {
                CostManager.Instance.ConsumeCost(cardData.cardCost);
                BattleCardManager.Instance.UseCard(this);
            }
            else
            {
                // 타일(또는 다른 상호작용 공간)이 아니라면 돌아가기
                rectTransform.anchoredPosition = _originAnchoredPos;
            }

            // 프리뷰 비활성화
            GridManager.Instance.OnEndDrag?.Invoke();
        }

        /// <summary>
        /// 카드를 타일에 오ㄹ렸을 때의 동작
        /// 타일에 올렸을 때 true 아닐 때 false
        /// </summary>
        protected abstract bool TryUseCard(RaycastHit hit);
        #endregion
    }
}
