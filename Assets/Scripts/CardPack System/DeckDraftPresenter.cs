using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Prototype.Card;
using UnityEngine.SceneManagement;


public class DeckDraftPresenter
{
    private DeckDraftView view;

    private List<CardDataSO> playerDeck = new List<CardDataSO>();

    private List<CardDataSO> displayCards = new List<CardDataSO>(new CardDataSO[10]);

    private int remainingRerolls = 3;        //교환 간으 횟수

    public System.Action OnComplete;

    public DeckDraftPresenter(DeckDraftView view)
    {
        this.view = view;

        this.view.OnSelectCard += ReplaceCard;

        this.view.OnConfirm += ConfirmDeck;
    }

    public void StartDraft()
    {
        //랜덤으로 30장 채우기
        GenerateRandomInitalDeck(30);

        //10장 추출
        PrepareDisplayCards();

        //UI 업데이트
        view.Show(displayCards, remainingRerolls);
    }

    //초기 30장 덱 생성
    private void GenerateRandomInitalDeck(int count)
    {
        playerDeck.Clear();
        for (int i = 0; i< count; i++)
        {
            CardType randomType = (CardType)Random.Range(0, 3);
            var card = StageManager.Instance.GetRandomCardData(randomType);
            if (card != null) playerDeck.Add(card);
        }
    }

    private void PrepareDisplayCards()
    {
        //유닛 6
        for (int i = 0; i < 6; i++) displayCards[i] = GetCardFromDeckByType(CardType.Unit);
        //아이템 2
        for (int i = 6; i < 8; i++) displayCards[i] = GetCardFromDeckByType(CardType.Item);
        //스펠 2
        for (int i = 8; i < 10; i++) displayCards[i] = GetCardFromDeckByType(CardType.Spell);
    }
    private CardDataSO GetCardFromDeckByType(CardType type)
    {
        // 덱에서 해당 타입의 첫 번째 카드를 찾아서 반환하고 덱에선 잠시 제거
        var card = playerDeck.FirstOrDefault(c => c.CardType == type);
        if (card != null) playerDeck.Remove(card);
        return card;
    }

    private void ReplaceCard(int index)
    {
        //횟수 체크하기
        if (remainingRerolls <= 0)
        {
            Debug.LogWarning("교환 횟수를 모두 사용했습니다!");
            return;
        }

        if (index < 0 || index >= displayCards.Count) return;

        CardDataSO oldCard = displayCards[index];

        //버린 카드 다시 풀로 반환
        StageManager.Instance.ReturnCardToPool(oldCard);

        //새로운 카드 풀에서 가져옴
        CardDataSO newCard = StageManager.Instance.GetRandomCardData(oldCard.CardType);

        if (newCard != null)
        {
            remainingRerolls--;
            displayCards[index] = newCard;
            Debug.Log($"[Draft] {oldCard.cardName} => {newCard.cardName} 교체 완료");
        }

        //UI 업데이트
        view.Refresh(displayCards, remainingRerolls);
    }

    public void ConfirmDeck()
    {
        if (DeckManager.Instance == null)
        {
            Debug.LogError("DeckManager 찾을 수 없긔");
            return;
        }


        // 교체 후 30장 완성
        foreach (var card in displayCards)
        {
            if (card != null) playerDeck.Add(card);
        }

        // 덱에 전달
        DeckManager.Instance.ownedDeck.Clear();
        foreach (var card in playerDeck)
        {
            DeckManager.Instance.TryAddCardToDeck(card, 1);
        }

        Debug.Log("덱 구성 완료");

        if (OnComplete != null)
        {
            OnComplete.Invoke();
        }
        else
        {
            Debug.LogWarning("연결안됐어용");
        }
    }

}
