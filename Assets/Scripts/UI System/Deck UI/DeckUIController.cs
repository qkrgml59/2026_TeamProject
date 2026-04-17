using Prototype.Card;
using System.Collections.Generic;
using UnityEngine;
using Utilitys;

namespace Game.UI
{
    public class DeckUIController : SingletonMonoBehaviour<DeckUIController>
    {
        [Header("팝업 뷰 참조")]
        public FullDeckListView fullDeckListView;
        public BattleDeckView battleDeckView;

        private void Start()
        {
            // View에 있는 버튼 클릭 이벤트에 토글 함수 연결
            if (fullDeckListView != null && fullDeckListView.toggleButton != null)
                fullDeckListView.toggleButton.onClick.AddListener(ToggleDeckListPopup);

            if (battleDeckView != null && battleDeckView.toggleButton != null)
                battleDeckView.toggleButton.onClick.AddListener(ToggleBattleDeckStatusPopup);

            HideAll();
        }

        public void HideAll()
        {
            if (fullDeckListView != null) fullDeckListView.SetActive(false);
            if (battleDeckView != null) battleDeckView.SetActive(false);
        }

        // --- 전체 덱 목록 팝업 토글 ---
        public void ToggleDeckListPopup()
        {
            if (fullDeckListView == null) return;

            if (fullDeckListView.IsActive)
            {
                // 열려있다면 닫기
                fullDeckListView.SetActive(false);
            }
            else
            {
                // 닫혀있다면 갱신 후 열기 (다른 팝업이 있다면 닫음)
                HideAll();
                RefreshDeckList();
                fullDeckListView.SetActive(true);
            }
        }

        private void RefreshDeckList()
        {
            fullDeckListView.Clear();
            int totalCount = 0;

            foreach (var entry in DeckManager.Instance.ownedDeck.Values)
            {
                for (int i = 0; i < entry.count; i++)
                {
                    fullDeckListView.AddCard(entry.cardData);
                    totalCount++;
                }
            }
            fullDeckListView.UpdateTitle(totalCount);
        }

        // --- 현재 덱 상황 팝업 토글 ---
        public void ToggleBattleDeckStatusPopup()
        {
            if (battleDeckView == null) return;

            if (battleDeckView.IsActive)
            {
                battleDeckView.SetActive(false);
            }
            else
            {
                HideAll();
                RefreshBattleDeckStatus();
                battleDeckView.SetActive(true);
            }
        }

        private void RefreshBattleDeckStatus()
        {
            battleDeckView.Clear();

            // 주의: BattleCardManager의 _deck과 _usedCards에 접근하기 위해 
            // 해당 변수들을 public으로 변경하거나, Getter 속성을 만들어주셔야 합니다.
            List<CardDataSO> drawPile = BattleCardManager.Instance.GetDeckCards();       // 예시 Getter
            List<CardDataSO> discardPile = BattleCardManager.Instance.GetUsedCards();    // 예시 Getter

            foreach (var card in drawPile) battleDeckView.AddDrawCard(card);
            foreach (var card in discardPile) battleDeckView.AddDiscardCard(card);

            battleDeckView.UpdateTitles(drawPile.Count, discardPile.Count);
        }

        private void Update()
        {
            // ESC로 팝업 닫기 (둘 중 하나라도 켜져있을 때)
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HideAll();
            }
        }
    }
}
