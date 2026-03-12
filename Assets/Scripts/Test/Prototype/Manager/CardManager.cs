using UnityEngine;
using Utilitys;
using System.Collections.Generic;
using System.Collections;

namespace Prototype.Card
{
    // 임시로 만든 매니저, 덱이랑 분리 필요.
    public class CardManager : SingletonMonoBehaviour<CardManager>
    {
        public List<CardDataSO> data = new();
        public CardBase cardPrefab;

        public int maxCardCount = 5;

        public List<CardBase> hands = new();

        public Canvas cardCanvas;

        private void Start()
        {
            BattleManager.Instance.OnRoundStart += OnRoundStart;
        }

        public void OnRoundStart()
        {
            StopAllCoroutines();
            StartCoroutine(DrawCards());
        }

        IEnumerator DrawCards()
        {
            while (hands.Count < 5)
            {
                DrawCard();
                yield return new WaitForSeconds(0.05f);
            }
        }

        public void UseCard(CardBase card)
        { 
            if(hands.Contains(card))
            {
                hands.Remove(card);
                CardSorting();
            }
        }

        void DrawCard()
        {
            CardBase card = Instantiate(cardPrefab, cardCanvas.transform);
            int rand = Random.Range(0, data.Count);
            card.Init(data[rand]);
            hands.Add(card);

            CardSorting();
        }

        void CardSorting()
        {
            float totalWidth = hands.Count * 170f;
            for (int i = 0; i < hands.Count; i++)
            {
                if(hands[i] != null)
                {
                    hands[i].rectTransform.anchoredPosition = new Vector3(-totalWidth / 2 + i * 170f, 45f, 0);
                }
            }
        }
    }
}
