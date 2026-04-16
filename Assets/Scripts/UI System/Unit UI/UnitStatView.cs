using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;

namespace Game.UI
{

    public class UnitStatView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("최종 벨류 설정")]public TextMeshProUGUI valueText;


        public event Action OnHover;
        public event Action OnHoverExit;

        public void UpdateView(string text)
        {
            if(valueText != null) valueText.text = text;
        }

        public void Clear()
        {
            if (valueText != null) valueText.text = "-";
        }


        // 마우스 감지 이벤트

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnHover?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnHoverExit?.Invoke();
        }
    }
}
