using UnityEngine;
using UnityEngine.EventSystems;


namespace UI
{
    /// <summary>
    /// 카드팩 슬롯 입력 처리
    /// </summary>
    public class PackSlot : MonoBehaviour, IPointerClickHandler
    {
        private int index;
        private PackSelectView view;

        public void Init(int index, PackSelectView view)
        {
            this.index = index;
            this.view = view;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                view.LeftClick(index);

            if (eventData.button == PointerEventData.InputButton.Right)
                view.RightClick(index);
        }
    }
}
