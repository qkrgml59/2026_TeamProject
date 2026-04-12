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
            if (data == null)
            {
                Debug.LogWarning("RewardView: data가 null임");
                return;
            }

            nameTmp.text = data.cardName;
            descriptionTmp.text = data.description;
            image.sprite = data.icon;
        }
    }
}
