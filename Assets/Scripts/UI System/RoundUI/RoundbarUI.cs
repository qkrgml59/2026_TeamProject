using UnityEngine;
using System.Collections.Generic;

namespace UI.Round
{
    public class RoundbarUI : MonoBehaviour
    {
        [SerializeField] private List<RoundSlotUI> slots;
        [SerializeField] private RectTransform indicator;

        private void Start()
        {
            Build();
            Refresh();
        }

        public void Build()
        {
            var cycle = StageManager.Instance.stageCycle;

            for (int i = 0; i < slots.Count; i++)
            {
                if (i < cycle.Count)
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

            MoveIndicator(cur);
        }

        private void MoveIndicator(int index)
        {
            if (index < 0) return;

            RectTransform target = slots[index].GetComponent<RectTransform>();

            indicator.position = target.position;
        }
    }
}

