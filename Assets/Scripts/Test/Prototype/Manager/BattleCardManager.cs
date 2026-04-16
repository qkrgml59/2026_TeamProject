using UnityEngine;
using Utilitys;
using System.Collections.Generic;
using System.Collections;

namespace Prototype.Card
{
    public enum BattleCardZone
    {
        Deck,       // 뽑기 전 카드 더미
        Hand,       // 손패
        Used        // 사용한 패 / 버린 패
    }

    public class BattleCardManager : SingletonMonoBehaviour<BattleCardManager>
    {
        [Header("컴포넌트 설정")]
        //public CardBase cardPrefab;
        public Canvas cardCanvas;

        [Header("핸드 설정")]
        public int defaultDrawCount = 2;
        public int maxHandCount = 9;

        public CardBase currentCastingCard { get; set; }            // 카드 무작위 버리기 할 때 본인이 버려지는걸 방지하기 위한 캐스팅 중인 카드 참조 변수 선언.

        [Header("현재 카드 구성")]
        [SerializeField] private List<CardDataSO> _deck = new();
        [SerializeField] private List<CardDataSO> _handCards = new();
        [SerializeField] private List<CardDataSO> _usedCards = new();

        [SerializeField] private List<CardBase> _handObjects = new();



        private void Start()
        {
            InitializeDeck();

            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.OnRoundStart += OnRoundStart;
            }

            if (DeckManager.Instance != null)
            {
                DeckManager.Instance.OnCardAdded += OnCardAdded;
                DeckManager.Instance.OnCardRemoved += OnCardRemoved;
            }
        }

        private void OnDestroy()
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.OnRoundStart -= OnRoundStart;
            }

            if (DeckManager.Instance != null)
            {
                DeckManager.Instance.OnCardAdded -= OnCardAdded;
                DeckManager.Instance.OnCardRemoved -= OnCardRemoved;
            }
        }

        public void InitializeDeck()
        {
            // 초기화 시 사용된 카드에 카드를 넣어 셔플 시킨다.
            foreach(CardEntry entry in DeckManager.Instance.ownedDeck.Values)
            {
                for (int i = 0; i < entry.count; i++)
                {
                    _usedCards.Add(entry.cardData);
                }
            }
        }

        public void OnRoundStart(RoundData round)
        {
            DrawCard(defaultDrawCount);
        }

        void OnCardAdded(CardDataSO cardData, int count)
        {
            for(int i = 0; i < count; i++)
            {
                AddBattleCard(cardData, BattleCardZone.Hand);
            }

        }

        void OnCardRemoved(CardDataSO cardData, int count)
        {
            for (int i = 0; i < count; i++)
            {
                RemoveBattleCard(cardData);
            }
        }

        /// <summary>
        /// 카드를 배틀 덱에 추가합니다.
        /// </summary>
        public void AddBattleCard(CardDataSO cardData, BattleCardZone zone)
        {
            if (cardData == null)
                return;

            switch(zone)
            {
                case BattleCardZone.Deck:
                    _deck.Add(cardData);
                    break;
                case BattleCardZone.Hand:
                    AddHand(cardData);       // 바로 카드 생성하면서 핸드에 추가
                    break;
                case BattleCardZone.Used:
                    _usedCards.Add(cardData);
                    break;
            }
        }

        /// <summary>
        /// 카드를 배틀 덱에서 제거합니다.
        /// </summary>
        public void RemoveBattleCard(CardDataSO cardData)
        {
            // 핸드 -> 사용된 카드 -> 덱 순서로 탐색후 제거

            if(_handCards.Contains(cardData))
            {
                for(int i = 0; i < _handObjects.Count; i++)
                {
                    if (_handObjects[i].cardData == cardData)
                    {
                        RemoveHand(_handObjects[i]);
                        break;
                    }
                }
            }
            else if (_usedCards.Contains(cardData)) _usedCards.Remove(cardData);
            else if(_deck.Contains(cardData)) _deck.Remove(cardData);
        }

        public void UseCard(CardBase card)
        { 
            if(!_handObjects.Contains(card))
            {
                Debug.LogError($"{card.cardData.name}가 실제 핸드에 없습니다.");
                return;
            }

            if (!_handCards.Contains(card.cardData))
            {
                Debug.LogError($"{card.cardData.name}가 핸드 데이터에 없습니다.");
                return;
            }

            // 사용한 카드로 이동
            _usedCards.Add(card.cardData);

            // 핸드에서 카드 제거
            RemoveHand(card);


            RefreshUI();
        }

        public void DrawCard(int count = 1)
        {
            if(count == 1)
                DrawCard();
            else
                StartCoroutine(DrawRoutine(count));
        }

        public void DrawCard()
        {
            if(_handCards.Count >= maxHandCount)
            {
                Debug.LogWarning("핸드가 가득 차 더 이상 카드를 뽑을 수 없습니다.");
                return;
            }

            if(_deck.Count == 0)
            {
                if(!Shuffle())
                {
                    StopAllCoroutines();
                    return;
                }
            }

            // 덱에서 첫번쨰 카드 핸드로 옮기고, 제거
            AddHand(_deck[0]);
            _deck.RemoveAt(0);
        }

        void AddHand(CardDataSO cardData)
        {
            if (cardData.cardPrefab == null)
            {
                Debug.LogError("데이터에 프리팹이 연결 안됨");
                return;
            }

            var card = Instantiate(cardData.cardPrefab, cardCanvas.transform);

            // 카드 초기화 및 핸드로 이동
            card.Init(cardData);
            _handCards.Add(cardData);
            _handObjects.Add(card);

            RefreshUI();
        }

        public void RemoveHand(CardBase card)
        {
            _handObjects.Remove(card);
            _handCards.Remove(card.cardData);
            Destroy(card.gameObject);
        }

        /// <summary>
        /// 손패에서 무작위로 amount개수의 카드를 버린다. (현재 캐스팅 중인 카드 제외)
        /// </summary>
        public bool DiscardRandomCard(int amount)
        {
            List<CardBase> validCards = new List<CardBase>();
            foreach (var card in _handObjects)
            {
                if (card != currentCastingCard)
                {
                    validCards.Add(card);
                }
            }

            if (validCards.Count < amount)
            {
                return false;
            }

            for (int i = 0; i < amount; i++)
            {
                // 무작위로 1장 선택
                int randIndex = Random.Range(0, validCards.Count);
                CardBase cardToDiscard = validCards[randIndex];

                _usedCards.Add(cardToDiscard.cardData);
                RemoveHand(cardToDiscard);

                validCards.RemoveAt(randIndex);
            }

            RefreshUI();
            return true;
        }

        IEnumerator DrawRoutine(int count = 1)
        {
            for(int i = 0; i < count; i++)
            {
                DrawCard();
                yield return new WaitForSeconds(0.05f);
            }
        }

        public bool Shuffle()
        {
            int shuffleCount = 0;
            while(_usedCards.Count > 0)
            {
                int rand = Random.Range(0, _usedCards.Count);
                CardDataSO data = _usedCards[rand];

                _usedCards.Remove(data);
                _deck.Add(data);

                shuffleCount++;
            }

            if(shuffleCount == 0)
            {
                Debug.LogWarning("셔플 실패 (덱 초기화 안 되어있음)");
                return false;
            }
            
            Debug.Log($"셔플 완료 ({shuffleCount}개)");
            return true;
        }

        void RefreshUI()
        {
            float totalWidth = _handObjects.Count * 170f;
            for (int i = 0; i < _handObjects.Count; i++)
            {
                if(_handObjects[i] != null)
                {
                    _handObjects[i].rectTransform.anchoredPosition = new Vector3(-totalWidth / 2 + i * 170f, 45f, 0);
                }
            }
        }
    }
}
