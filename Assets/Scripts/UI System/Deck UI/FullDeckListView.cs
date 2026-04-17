using Prototype.Card;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI
{
    public class FullDeckListView : MonoBehaviour
    {
        [Header("UI 요소")]
        public TextMeshProUGUI titleText;
        public Button toggleButton;
        public Transform contentArea;
        public CardUIView cardUIPrefab;

        public bool IsActive => gameObject.activeSelf;
        public void SetActive(bool isActive) => gameObject.SetActive(isActive);

        public void UpdateTitle(int count)
        {
            if (titleText != null) titleText.text = $"< 전체 덱 목록 : {count} >";
        }

        public void Clear()
        {
            foreach (Transform child in contentArea)
            {
                Destroy(child.gameObject);
            }
        }

        public void AddCard(CardDataSO cardData)
        {
            CardUIView newCard = Instantiate(cardUIPrefab, contentArea);
            newCard.Init(cardData);
        }
    }
}
