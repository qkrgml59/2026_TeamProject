using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Prototype.Card;
using UnityEngine.SceneManagement;

namespace Title.UI
{
    public class DeckDraftPresenter
    {
        private DeckDraftView view;

        private List<CardDataSO> playerDeck = new List<CardDataSO>();

        private HashSet<int> selectedIndices = new HashSet<int>(); // 선택된 카드 번호들

        private int remainingRerolls = 2;        //교환 간으 횟수
        private int hoveredIndex = -1;           //마우스가 올라가 있는 카드 번호

        public System.Action OnComplete;

        public DeckDraftPresenter(DeckDraftView view)
        {
            this.view = view;

            this.view.OnCardClick += HandleCardClick;
            this.view.OnCardHover += HandleCardHover;
            this.view.OnReplace += ReplaceSelectedCards;
            this.view.OnConfirm += ConfirmDeck;

        }

        public void StartDraft()
        {
            //10장 생성
            GenerateRandomInitalDeck(6, 2, 2);

            //UI 업데이트
            view.Show(playerDeck, remainingRerolls);
        }

        //초기 덱 완성
        private void GenerateRandomInitalDeck(int unit, int item, int spell)
        {
            if (StageManager.Instance == null) return;

            playerDeck.Clear();

            // 타입별로 랜덤 카드 가져오기
            for (int i = 0; i < unit; i++) AddCardToDeck(CardType.Unit);
            for (int i = 0; i < item; i++) AddCardToDeck(CardType.Item);
            for (int i = 0; i < spell; i++) AddCardToDeck(CardType.Spell);
        }

        private void AddCardToDeck(CardType type)
        {
            CardDataSO card = StageManager.Instance.GetRandomCardData(type);
            if (card != null) playerDeck.Add(card);
        }

        private void HandleCardClick(int index, int btnType)
        {
            if (remainingRerolls <= 0) return;

            if (btnType == 0)         //좌클릭 선택
            {
                if (selectedIndices.Contains(index))
                    selectedIndices.Remove(index);          //이미 선택도ㅒㅆ다면 해제
                else
                    selectedIndices.Add(index);             //아니라면 추가
            }
            else
            {
                selectedIndices.Remove(index);
            }

            RefreshSingleCardVisual(index);
            view.UpdateRerollUI(remainingRerolls, selectedIndices.Count > 0);
        }

       private void HandleCardHover(int index, bool isHover)
       {
            hoveredIndex = isHover ? index : -1;
            RefreshSingleCardVisual(index);
       }

        private void RefreshSingleCardVisual(int index)
        {
            if (index < 0 || index >= playerDeck.Count) return;

            bool isSelected = selectedIndices.Contains(index);
         
            float scale = (index == hoveredIndex) ? 1.05f : 1.0f;

            view.UpdateCardVisual(index, isSelected, scale);
        }

        private void ReplaceSelectedCards()
        {
            if (remainingRerolls <= 0 || selectedIndices.Count == 0) return;

            //새 카드 먼저 뽑아옴
            List<CardDataSO> newCards = new List<CardDataSO>();
            foreach(int idx in selectedIndices)
            {
                newCards.Add(StageManager.Instance.GetRandomCardData(playerDeck[idx].CardType));
            }

            //기존 카드 풀에 넣고 새 카드 교체
            int i = 0;
            foreach (int idx in selectedIndices)
            {
                StageManager.Instance.ReturnCardToPool(playerDeck[idx]);
                playerDeck[idx] = newCards[i];
                i++;
            }

            //교환 후 데이터 정리
            remainingRerolls--;
            selectedIndices.Clear();

            view.Show(playerDeck, remainingRerolls);
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
}


