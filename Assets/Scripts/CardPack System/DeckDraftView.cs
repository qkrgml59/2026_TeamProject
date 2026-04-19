using Prototype.Card;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 덱 드래프트 UI
/// </summary>
public class DeckDraftView : MonoBehaviour
{
    public Action<int> OnSelectCard;
    public Action OnConfirm;

    [Header("UI")]
    public Transform slotRoot;
    public GameObject cardSlotPrefab;
    public Button confirmButton;
    [SerializeField] private TextMeshProUGUI rerollText;

    private List<PreviewCardSlot> activeSlots = new List<PreviewCardSlot>();

    private void Awake()
    {
        if (confirmButton != null)
            confirmButton.onClick.AddListener(() => OnConfirm?.Invoke());
    }

    public void Show(List<CardDataSO> cards, int rerollCount)
    {
        UpdateRerollUI(rerollCount);

        foreach (Transform child in slotRoot) Destroy(child.gameObject);
        activeSlots.Clear();

        for (int i = 0; i < cards.Count; i++)
        {
            int index = i; //버그 방지... 클로저 방지라는데 클로저가 뭘까요
            GameObject go = Instantiate(cardSlotPrefab, slotRoot);

            if (go.TryGetComponent(out PreviewCardSlot slot))
            {
                //버튼 연결
                Button btn = go.GetComponent<Button>();
                if (btn == null) btn = go.AddComponent<Button>();

                btn.onClick.AddListener(() => OnSelectCard?.Invoke(index));

                // 데이터 바인딩
                slot.Init(cards[i], 1);
                activeSlots.Add(slot);
            }
        }
    }

    //TODO : 여기도 카드팩 선택 처럼 연출 넣기
    //교체 카드 UI
    public void Refresh(List<CardDataSO> cards, int rerollCount)
    {
        UpdateRerollUI(rerollCount);

        for (int i = 0; i < cards.Count; i++)
        {
            if (i < activeSlots.Count)
            {
                activeSlots[i].Init(cards[i], 1);
            }
        }
    }

    private void UpdateRerollUI(int count)
    {
        if (rerollText != null)
            rerollText.text = $"남은 교환 횟수 : {count}";

        
        if (count <= 0)
        {
            //텍스트 색 변경
            rerollText.color = Color.red;
        }
    }
}
