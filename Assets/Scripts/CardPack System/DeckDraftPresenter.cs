using Prototype.Card;
using System.Collections.Generic;



public class DeckDraftPresenter
{
    private DeckDraftView view;

    private List<CardDataSO> deck = new();
    private List<CardDataSO> selected = new();

    public System.Action OnComplete;

    public DeckDraftPresenter(DeckDraftView view)
    {
        this.view = view;
        view.OnSelectCard += SelectCard;
    }

    public void Init()
    {
        GenerateDeck();
        view.Show(deck);
    }

    /// <summary>
    /// 임의로 우선 30장 생성
    /// </summary>
    private void GenerateDeck()
    {
        deck.Clear();

        for (int i = 0; i < 30; i++)
        {
            var card = StageManager.Instance.GetRandomCardData(CardType.Unit);
            if (card != null)
                deck.Add(card);
        }
    }

    /// <summary>
    /// 카드 선택 
    /// </summary>
    private void SelectCard(int index)
    {
        if (selected.Contains(deck[index]))
            return;

        selected.Add(deck[index]);

        if (selected.Count >= 3)
        {
            ReplaceCards();
            OnComplete?.Invoke();
        }
    }

    /// <summary>
    /// 카드 교체 로직
    /// </summary>
    private void ReplaceCards()
    {
        foreach (var card in selected)
        {
            StageManager.Instance.ReturnCardToPool(card);

            var newCard = StageManager.Instance.GetRandomCardData(card.CardType);
            int idx = deck.IndexOf(card);

            if (idx >= 0)
                deck[idx] = newCard;
        }

        view.Refresh(deck);
    }
}
