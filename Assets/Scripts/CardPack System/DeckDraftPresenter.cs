using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Prototype.Card;
using UnityEngine.SceneManagement;


public class DeckDraftPresenter
{
    private DeckDraftView view;

    private List<CardDataSO> playerDeck = new List<CardDataSO>();

    private int remainingRerolls = 2;        //교환 간으 횟수

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
        GenerateRandomInitalDeck(6, 2, 2);

        //UI 업데이트
        view.Show(playerDeck, remainingRerolls);
    }

    //초기 덱 완성
    private void GenerateRandomInitalDeck(int unit, int item, int spell)
    {
        if (StageManager.Instance == null) return;

        playerDeck.Clear();

        // 유닛 추가
        for (int i = 0; i< unit; i++)
        {
            CardDataSO card = StageManager.Instance.GetRandomCardData(CardType.Unit);
            if (card != null)
                playerDeck.Add(card);
        }

        // 아이템 추가
        for (int i = 0; i < item; i++)
        {
            CardDataSO card = StageManager.Instance.GetRandomCardData(CardType.Item);
            if (card != null)
                playerDeck.Add(card);
        }

        // 스펠 추가
        for (int i = 0; i < spell; i++)
        {
            CardDataSO card = StageManager.Instance.GetRandomCardData(CardType.Spell);
            if (card != null)
                playerDeck.Add(card);
        }
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

        if (index < 0 || index >= playerDeck.Count) return;

        CardDataSO oldCard = playerDeck[index];


        // 동일 카드가 나올 확률을 줄이고자 새로운 카드 먼저 받아오기
        //새로운 카드 풀에서 가져옴
        CardDataSO newCard = StageManager.Instance.GetRandomCardData(oldCard.CardType);

        //버린 카드 다시 풀로 반환
        StageManager.Instance.ReturnCardToPool(oldCard);


        if (newCard != null)
        {
            remainingRerolls--;
            playerDeck[index] = newCard;
            Debug.Log($"[Draft] {oldCard.cardName} => {newCard.cardName} 교체 완료");
        }

        //UI 업데이트
        view.Refresh(playerDeck, remainingRerolls);
    }

    public void ConfirmDeck()
    {
        if (DeckManager.Instance == null)
        {
            Debug.LogError("DeckManager 찾을 수 없긔");
            return;
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
