using UnityEngine;
using Utilitys;
using System.Collections.Generic;
using System.Collections;

namespace Prototype.Card
{
    public class CardManager : SingletonMonoBehaviour<CardManager>
    {
        [Header("컴포넌트 설정")]
        public CardBase cardPrefab;
        public Canvas cardCanvas;

        [Header("핸드 설정")]
        public int defaultDrawCount = 2;
        public int maxHandCount = 9;

        [Header("현재 카드 구성")]
        [SerializeField] private List<CardDataSO> _deck = new();
        [SerializeField] private List<CardDataSO> _hand = new();
        [SerializeField] private List<CardDataSO> _used = new();

        [SerializeField] private List<CardBase> _handObjects = new();



        private void Start()
        {
            InitializeDeck();

            BattleManager.Instance.OnRoundStart += OnRoundStart;
        }

        public void InitializeDeck()
        {
            // 초기화 시 사용된 카드에 카드를 넣어 셔플 시킨다.
            foreach(CardEntry entry in DeckManager.Instance.ownedDeck.Values)
            {
                for (int i = 0; i < entry.count; i++)
                {
                    _used.Add(entry.cardData);
                }
            }
        }

        public void OnRoundStart()
        {
            DrawCard(defaultDrawCount);
        }


        public void UseCard(CardBase card)
        { 
            if(!_handObjects.Contains(card))
            {
                Debug.LogError($"{card.cardData.name}가 실제 핸드에 없습니다.");
                return;
            }

            if (!_hand.Contains(card.cardData))
            {
                Debug.LogError($"{card.cardData.name}가 핸드 데이터에 없습니다.");
                return;
            }

            // 사용한 카드로 이동
            _used.Add(card.cardData);

            // 핸드에서 카드 제거
            _handObjects.Remove(card);
            _hand.Remove(card.cardData);
            Destroy(card.gameObject);


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
            if(_hand.Count >= maxHandCount)
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

            // 카드 오브젝트 생성
            CardBase card = Instantiate(cardPrefab, cardCanvas.transform);

            // 덱에서 첫번쨰 카드 꺼내오기
            CardDataSO data = _deck[0];
            _deck.Remove(data);

            // 카드 초기화 및 핸드로 이동
            card.Init(data);
            _hand.Add(data);
            _handObjects.Add(card);

            RefreshUI();
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
            while(_used.Count > 0)
            {
                int rand = Random.Range(0, _used.Count);
                CardDataSO data = _used[rand];

                _used.Remove(data);
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
