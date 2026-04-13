using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class ItemSlotView : MonoBehaviour
    {
        [SerializeField] private Image iconImage;

        public void Show(Sprite iconSprite)
        {
            gameObject.SetActive(true);
            if (iconSprite != null)
            {
                iconImage.sprite = iconSprite;
                //iconImage.enabled = true;
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            iconImage.sprite = null;
            //iconImage.enabled = false;
        }
    }
}
