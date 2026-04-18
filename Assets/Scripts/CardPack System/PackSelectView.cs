using UnityEngine;
using System.Collections.Generic;
using System;
using Prototype.Card;

namespace UI
{
    public class PackSelectView : MonoBehaviour
    {
        public System.Action<int> OnLeftClickPack;
        public System.Action<int> OnRightClickPack;

        [SerializeField] private PackSlot[] slots;

        public void Bind(List<StageData> packs)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].Init(i, this);
                slots[i].gameObject.SetActive(i < packs.Count);
            }
        }

        public void LeftClick(int index)
        {
            OnLeftClickPack?.Invoke(index);
        }

        public void RightClick(int index)
        {
            OnRightClickPack?.Invoke(index);
        }

        /// <summary>
        /// 우클릭 시 카드 미리보기 표시
        /// </summary>
        public void ShowDetail(List<CardDataSO> cards)
        {
            // UI 표시 로직
            // 예: 카드 프리팹 생성 or 텍스트 업데이트

            Debug.Log($"카드 {cards.Count}장 미리보기 표시");
        }
    }
}

