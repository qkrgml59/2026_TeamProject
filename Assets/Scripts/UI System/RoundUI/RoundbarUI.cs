using UnityEngine;
using System.Collections.Generic;

namespace UI.Round
{
    public class RoundbarUI : MonoBehaviour
    {
        [Header("슬롯 프리팹")]
        [SerializeField] private RoundSlotUI slotPrefab;

        [Header("슬롯 부모")]
        [SerializeField] private Transform slotParent;

        // 생성된 슬롯 저장
        private List<RoundSlotUI> slots = new List<RoundSlotUI>();

        private void Start()
        {
            Build();
        }

        public void Build()
        {
            var cycle = StageManager.Instance.stageCycle;

            for (int i = 0; i < slots.Count; i++)
            {
                Destroy(slots[i].gameObject);
            }

            slots.Clear();

            // 라운드 개수만큼 생성
            for (int i = 0; i < cycle.Count; i++)
            {
                // 슬롯 생성
                RoundSlotUI slot =
                    Instantiate(
                        slotPrefab,
                        slotParent);

                // 타입 설정
                slot.SetData(cycle[i]);

                // 리스트 저장
                slots.Add(slot);
            }

            Refresh();
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

