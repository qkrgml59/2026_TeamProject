using UnityEngine;
using System.Collections.Generic;

namespace UI.Round
{
    public class RoundbarUI : MonoBehaviour
    {
        [SerializeField] private List<RoundSlotUI> slots;

        private void Start()
        {
            Build();
        }

        public void Build()
        {
            var cycle = StageManager.Instance.stageCycle;

            for (int i = 0; i < slots.Count; i++)
            {
                // 사용 안 하는 슬롯 숨기기
                if (i >= cycle.Count)
                {
                    slots[i].gameObject.SetActive(false);
                    continue;
                }

                slots[i].gameObject.SetActive(true);

                // 슬롯 타입 설정
                slots[i].SetData(cycle[i]);
            }
        }

        public void Refresh()
        {
            int cur = StageManager.Instance.CurRoundIndex;

            for (int i = 0; i < slots.Count; i++)
            {
                bool isCurrent = (i == cur);
                bool isPassed = (i < cur);

                slots[i].SetState(isCurrent, isPassed);
            }

        }

    }
}

