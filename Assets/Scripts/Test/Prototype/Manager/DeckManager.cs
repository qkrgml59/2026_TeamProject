using UnityEngine;
using Utilitys;
using System.Collections.Generic;

namespace Prototype.Card
{
    public class DeckManager : SingletonMonoBehaviour<DeckManager>
    {
        [Header("덱 정보")]
        [SerializeField]private List<CardDataSO> _testDeck = new();       // 초기 덱 설정을 위한 테스트 목록
        public Dictionary<string, CardEntry> ownedDeck { get; private set; } = new ();

        [Header("덱 설정")]
        public int maxDeckSize = 40;            // 덱 최대/최소 설정 (사용 하려나요)
        public int minDeckSize = 20;

        private void Start()
        {
            InitDeck();
        }

        [ContextMenu("덱 초기화")]
        public void InitDeck()
        {
            ownedDeck.Clear();
            foreach (var card in _testDeck)
            {
                if(!TryAddCard(card))
                {
                    Debug.LogError("덱 초기화 실패");
                    return;
                }
            }
        }

        /// <summary>
        ///  덱에 카드 추가를 시도합니다.
        /// </summary>
        public bool TryAddCard(CardDataSO newCard, int count = 1)
        {
            int totalCount = GetTotalCardCount();
            if (totalCount >= maxDeckSize)
            {
                Debug.LogWarning("덱이 가득 차 카드를 추가 할 수 없습니다.");
                return false;
            }

            if (totalCount + count > maxDeckSize)
            {
                Debug.LogWarning("덱에 공간이 부족해 카드를 추가할 수 없습니다.");
                return false;
            }

            if (ownedDeck.ContainsKey(newCard.cardId))
            {
                // 이미 덱에 존재 하는 카드면 개수 추가
                ownedDeck[newCard.cardId].count += count;       // TODO: 카드 개수 직접 변경하지 않도록 수정 필요.
            }
            else
            {
                ownedDeck[newCard.cardId] = new CardEntry(newCard, count);
            }

            // TODO: CardManager 쪽에도 카드 추가
            return true;
        }

        public bool TryRemoveCard(CardDataSO targetCard, int count)
        {
            int totalCount = GetTotalCardCount();
            if (totalCount <= minDeckSize)
            {
                Debug.LogWarning($"카드는 최소 {minDeckSize}장 있어야 합니다.");
                return false;
            }

            if (totalCount - count < minDeckSize)
            {
                Debug.LogWarning($"카드를 {totalCount - minDeckSize}장 까지만 버릴 수 있습니다.");
                return false;
            }

            if (ownedDeck.ContainsKey(targetCard.cardId))
            {
                if(ownedDeck[targetCard.cardId].count < count)
                {
                    Debug.LogWarning("카드는 현재 보유중인 개수 보다 많이 버릴 수 없습니다.");
                    return false;
                }

                // 이미 덱에 존재 하는 카드면 개수 추가
                ownedDeck[targetCard.cardId].count -= count;       // TODO: 카드 개수 직접 변경하지 않도록 수정 필요.

                // 남은 카드가 없으면 엔트리 제거
                if(ownedDeck[targetCard.cardId].count == 0)
                    ownedDeck.Remove(targetCard.cardId);
            }
            else
            {
                Debug.LogWarning("현재 해당 카드를 보유하고 있지 않습니다.");
                return false;
            }

            // TODO: CardManager 쪽에서도 카드 제거
            return true;
        }

        #region Getters

        // 현재 덱의 총 카드 개수를 반환합니다.
        public int GetTotalCardCount()
        {
            int total = 0;
            foreach (var entry in ownedDeck.Values)
                total += entry.count;

            return total;
        }

        #endregion
    }
}
