using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Title.UI
{
    public class PointerDetector : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Action<int> OnPointerClickEvent;
        public Action<bool> OnPointerHoverEvent;

        public void OnPointerClick(PointerEventData eventData)
        {
            int type = (eventData.button == PointerEventData.InputButton.Left) ? 0 : 1;
            OnPointerClickEvent?.Invoke(type);
        }
        public void OnPointerEnter(PointerEventData eventData) => OnPointerHoverEvent?.Invoke(true);
        public void OnPointerExit(PointerEventData eventData) => OnPointerHoverEvent?.Invoke(false);
    }

}

