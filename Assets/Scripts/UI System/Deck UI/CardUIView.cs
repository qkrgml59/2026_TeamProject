using Prototype.Card;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class CardUIView : MonoBehaviour             // 정보만 보여줄 CardUI
    {
        public Image icon;
        public TextMeshProUGUI cardName;
        public TextMeshProUGUI description;
        public TextMeshProUGUI costText;

        public void Init(CardDataSO cardData)
        {
            if (cardData == null) return;

            if (icon != null) icon.sprite = cardData.icon;
            if (cardName != null) cardName.text = cardData.cardName;
            if (description != null) description.text = cardData.description;
            if (costText != null) costText.text = cardData.cost.ToString();
        }
    }
}
