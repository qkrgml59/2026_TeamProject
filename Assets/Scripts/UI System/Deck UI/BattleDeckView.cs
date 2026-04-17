using Prototype.Card;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class BattleDeckView : MonoBehaviour
    {
        public Button toggleButton;
        public CardUIView cardUIPrefab;

        [Header("뽑을 카드")]
        public TextMeshProUGUI drawTitleText;
        public Transform drawContentArea;

        [Header("버린 카드")]
        public TextMeshProUGUI discardTitleText;
        public Transform discardContentArea;

        public bool IsActive => gameObject.activeSelf;
        public void SetActive(bool isActive) => gameObject.SetActive(isActive);

        public void UpdateTitles(int drawCount, int discardCount)
        {
            if (drawTitleText != null) drawTitleText.text = $"< 뽑을 카드 : {drawCount} >";
            if (discardTitleText != null) discardTitleText.text = $"< 버린 카드 : {discardCount} >";
        }

        public void Clear()
        {
            foreach (Transform child in drawContentArea) Destroy(child.gameObject);
            foreach (Transform child in discardContentArea) Destroy(child.gameObject);
        }

        public void AddDrawCard(CardDataSO cardData)
        {
            Instantiate(cardUIPrefab, drawContentArea).Init(cardData);
        }

        public void AddDiscardCard(CardDataSO cardData)
        {
            Instantiate(cardUIPrefab, discardContentArea).Init(cardData);
        }
    }
}
