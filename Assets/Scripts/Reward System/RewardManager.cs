using Prototype.Card;
using System;
using System.Collections.Generic;
using Test.UI;
using UnityEngine;
using Utilitys;

namespace UI
{
    public class RewardManager : SingletonMonoBehaviour<RewardManager>
    {
        [Header("UI")]
        public Canvas canvas;              //보상 UI 전체 캔버스
        public RewardView[] views;          //보상 슬롯 3개

        private bool isSelecting = false;

        private List<CardDataSO> items = new List<CardDataSO>();

        public Action<CardDataSO> OnSelectReward;
        public Action OnSkip;

        //전투 종료 후 진입
        public void StartRewardPhase()
        {
            isSelecting = false;

            GenerateRewards();
            ShowRewardSelectionUI();
        }

        //보상 3개 생성
        private void GenerateRewards()
        {
            items.Clear();

            items.Add(StageManager.Instance.GetRandomCardData(CardType.Unit));
            items.Add(StageManager.Instance.GetRandomCardData(CardType.Item));
            items.Add(StageManager.Instance.GetRandomCardData(CardType.Spell));
        }

        //보상 UI 표시
        private void ShowRewardSelectionUI()
        {
            for (int i = 0; i < views.Length; i++)
            {
                if (i < items.Count && items[i] != null)
                {
                    views[i].gameObject.SetActive(true);
                    views[i].Bind(items[i], i, OnRewardSelected);
                }
                else
                {
                    views[i].gameObject.SetActive(false);
                }
            }

            canvas.enabled = true;
        }

        //보상 선택
        public void OnRewardSelected(int index)
        {
            if (isSelecting) return;
            isSelecting = true;

            if (index < 0 || index >= items.Count)
            {
                Debug.LogError($"잘못된 index 접근: {index}");
                return;
            }

            CardDataSO selected = items[index];

            //선택 안 된 카드들 복구
            for (int i = 0; i < items.Count; i++)
            {
                if (i == index) continue;

                if (items[i] != null)
                    StageManager.Instance.ReturnCardToPool(items[i]);    
            }

            OnSelectReward?.Invoke(selected);

            Clear();
            canvas.enabled = false;
        }

        public void Skip()
        {
           for (int i = 0; i<items.Count; i++)
           {
                if (items[i] != null)
                    StageManager.Instance.ReturnCardToPool(items[i]);
           }

            canvas.enabled = false;

            OnSkip?.Invoke();

            Clear();
        }

        private void Clear()
        {
            items.Clear();
        }
    }
}

