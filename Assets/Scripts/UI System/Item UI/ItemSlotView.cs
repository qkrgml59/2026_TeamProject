using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class ItemSlotView : MonoBehaviour
    {
        [SerializeField] private Image iconImage;

        public void Show(Sprite iconSprite)
        {
            if (iconSprite != null)
            {
                iconImage.sprite = iconSprite;
                iconImage.enabled = true;
            }
        }

        public void Hide()
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }
}
