using Prototype.Card;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RewardView : MonoBehaviour
    {
        public TextMeshProUGUI nameTmp;
        public TextMeshProUGUI descriptionTmp;
        public Image image;

        public void Show(CardDataSO data)
        {
            nameTmp.text = data.cardName;
            descriptionTmp.text = data.description;
            image.sprite = data.icon;
        }
    }
}
