using UnityEngine;
using UnityEngine.UI;
namespace UI.Round
{
    //그냥 궁금해 진건데 라운드 색상을 굳이 내가 바꿔야 하나
    //아트에서 수정을 하는 건지.. 무너지.. 모르겠다..
    //일단 만들기.

    public class RoundSlotUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private GameObject highlight;
        [SerializeField] private GameObject cleared;

        public void SetData(RoundType type)
        {
            //타입별 색, 아이콘 변경
            switch (type)
            {
                case RoundType.Normal:
                    icon.color = Color.green;
                    break;
                case RoundType.Elite:
                    icon.color = Color.orange;
                    break;
                case RoundType.Boss:
                    icon.color = Color.red;
                    break;
                case RoundType.Rest:
                    icon.color = Color.gray;
                    break;


            }
        }
        public void SetState(bool isCurrent, bool isPassed)
        {
            highlight.SetActive(isCurrent);
            cleared.SetActive(isPassed);
        }
    }
}

