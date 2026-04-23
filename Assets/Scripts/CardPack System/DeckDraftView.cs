using Prototype.Card;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace Title.UI
{
    /// <summary>
    /// 덱 드래프트 UI
    /// </summary>
    public class DeckDraftView : MonoBehaviour
    {
        public Action<int, int> OnCardClick;
        public Action<int, bool> OnCardHover;
        public Action OnReplace;
        public Action OnConfirm;

        [Header("UI")]
        public Transform slotRoot;
        public GameObject cardSlotPrefab;
        public Button replaceButton;
        public Button confirmButton;
        [SerializeField] private TextMeshProUGUI rerollText;

        private List<DraftCardSlot> activeDraftSlots = new List<DraftCardSlot>();
        private List<GameObject> selectionVisuals = new List<GameObject>();

        private void Awake()
        {
            replaceButton.onClick.AddListener(() => OnReplace?.Invoke());
            confirmButton.onClick.AddListener(() => OnConfirm?.Invoke());

            AddButtonHoverEffect(replaceButton);
            AddButtonHoverEffect(confirmButton);
        }

        public void Show(List<CardDataSO> cards, int rerollCount)
        {
            UpdateRerollUI(rerollCount, false);

            foreach (Transform child in slotRoot) Destroy(child.gameObject);
            activeDraftSlots.Clear();


            for (int i = 0; i < cards.Count; i++)
            {
                int index = i; //버그 방지... 클로저 방지라는데 클로저가 뭘까요
                GameObject go = Instantiate(cardSlotPrefab, slotRoot);

                DraftCardSlot draftSlot = go.GetComponent<DraftCardSlot>();

                if (draftSlot !=null)
                {
                    draftSlot.Init(cards[i], 1);
                    activeDraftSlots.Add(draftSlot);

                    var detector = go.GetComponent<PointerDetector>() ?? go.AddComponent<PointerDetector>();
                    detector.OnPointerClickEvent = (btnType) => OnCardClick?.Invoke(index, btnType);
                    detector.OnPointerHoverEvent = (isHover) => OnCardHover?.Invoke(index, isHover);

                }
            }
            UpdateRerollUI(rerollCount, false);
        }

        public void UpdateCardVisual(int index, bool isSelected, float scale)
        {
            if (index < 0 || index >= activeDraftSlots.Count) return;

            // 크기 조절 (호버 연출)
            activeDraftSlots[index].transform.localScale = Vector3.one * scale;

            // 선택 연출 (DraftCardSlot의 함수 호출)
            activeDraftSlots[index].SetSelect(isSelected);

        }

        public void UpdateRerollUI(int count, bool hasSelection)
        {
            if (rerollText != null)
                rerollText.text = $"남은 교환 횟수 : {count} /2 ";

            //횟수 남을때랑 선택한 카드가 있을 때 
            replaceButton.interactable = (count > 0 && hasSelection);

            rerollText.color= (count <= 0) ? Color.red : Color.black;
            replaceButton.GetComponent<Image>().color = (count <=0) ? Color.white : Color.gray;
        }

        private void AddButtonHoverEffect(Button btn)
        {
            //?? -> 내 왼쪽이  Null이면 오른쪽을 실행해!!! 라는 뜻
            var detector = btn.gameObject.GetComponent<PointerDetector>() ?? btn.gameObject.AddComponent<PointerDetector>();
            detector.OnPointerHoverEvent = (isHover) =>
            {
                if (btn.interactable)
                    btn.transform.localScale = isHover ? Vector3.one * 1.1f : Vector3.one;
            };
        }
    }
}




