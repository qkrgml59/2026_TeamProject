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

        //현재 표시 중인 보상 데이터
        private CardDataSO[] items = new CardDataSO[3];

        // 각 카드가 어떤 풀에서 왔는지 저장
        private List<CardDataSO>[] originPools = new List<CardDataSO>[3];

        public Action<CardDataSO> OnSelectReward;
        public Action OnSkip;

        //전투 종료 후 진입
        public void StartRewardPhase()
        {
            GenerateRewards();
            ShowRewardSelectionUI();
        }

        //보상 3개 생성
        private void GenerateRewards()
        {
            items[0] = StageManager.Instance.GetRandomCardData(CardType.Unit);
            //items[1] = StageManager.Instance.GetRandomCardData(CardType.Item);
            items[1] = StageManager.Instance.GetRandomCardData(CardType.Unit);
            items[2] = StageManager.Instance.GetRandomCardData(CardType.Spell);

            //어떤 풀에서 나왔는지
            originPools[0] = StageManager.Instance.UnitPool;
            // originPools[1] = StageManager.Instance.ItemPool;
            originPools[1] = StageManager.Instance.UnitPool;
            originPools[2] = StageManager.Instance.SpellPool;
        }

        //보상 UI 표시
        private void ShowRewardSelectionUI()
        {
            for (int i = 0; i< views.Length; i++)
            {
                views[i].Bind(items[i], i, OnRewardSelected);
            }

            canvas.enabled = true;
        }

        //보상 선택
        public void OnRewardSelected(int index)
        {
            CardDataSO selected = items[index];

            // 선택 안 된 카드들은 다시 복구
            for (int i = 0; i< items.Length; i++)
            {
                if (i == index) continue;

                if (items[i] != null)
                    StageManager.Instance.ReturnCardToPool(items[i], originPools[i]);
            }

            Clear();
            canvas.enabled = false;

            //외부 전달
            OnSelectReward?.Invoke(selected);

        }

        public void Skip()
        {
            for (int i = 0; i< items.Length; i++)
            {
                if (items[i] != null)
                    StageManager.Instance.ReturnCardToPool(items[i], originPools[i]);
            }

            Clear();
            canvas.enabled = false;

            OnSkip?.Invoke();
        }

        private void Clear()
        {
            for (int i = 0; i< items.Length; i++)
            {
                items[i] = null;
                originPools[i] = null;
            }
        }
    }
}

