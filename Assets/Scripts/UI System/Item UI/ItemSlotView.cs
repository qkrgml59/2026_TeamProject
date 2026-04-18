using Item;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    public class ItemSlotView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image iconImage;
        ItemBase item;

        public void Show(ItemBase item)
        {
            if(item == null)
            { 
                Hide();
                return;
            }

            this.item = item;

            gameObject.SetActive(true);
            if (item.itemData != null)
            {
                iconImage.sprite = item.itemData.icon;
                //iconImage.enabled = true;
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            iconImage.sprite = null;
            //iconImage.enabled = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // TODO : 우클릭 조건 추가?

            if(DetailPanelController.Instance != null&&
                item != null && item.itemData != null)
            {
                // TODO : 이벤트 방식으로 변경하기
                DetailPanelController.Instance.ShowItemDetail(item.itemData);
            }
        }
    }
}
