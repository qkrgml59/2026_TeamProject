using Game.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class EmptyClickArea : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            // 아무런 클릭이 되지 않은 경우 UI를 끄는 임시 코드
            if(DetailPanelController.Instance != null)
            {
                DetailPanelController.Instance.HideAll();
            }
        }
    }
}
