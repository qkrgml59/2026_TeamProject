using StatSystem;
using Unit;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{

    public class UnitResourceView : MonoBehaviour
    {
        [Header("현재 자원 표시")]
        public Image curResource;

        [Header("자원 색상")]
        public Color manaColor;     // 마나 색상
        public Color rageColor;     // 분노 색상


        // 색상 변경, 약탈 등을 당했을때 대비
        private ResourceType preType;

        public void SetResourceColor(ResourceType type)
        {
            if (curResource == null) return;

            switch(type)
            {
                case ResourceType.None:
                    curResource.color = Color.clear;            // 자원을 사용하지 않는 경우 투명
                    break;
                case ResourceType.Mana:
                    curResource.color = manaColor;
                    break;
                case ResourceType.Rage:
                    curResource.color = rageColor;
                    break;
            }
        }

        public void OnResourceChanged(ResourceInfo info)
        {
            if (curResource == null) return;

            if(info.resourceType != preType)
            {
                // 기존과 타입이 다른 경우 색상 변경
                preType = info.resourceType;
                SetResourceColor(info.resourceType);
            }

            float ratio = info.ResourceRatio;
            curResource.fillAmount = ratio;
        }
    }
}
