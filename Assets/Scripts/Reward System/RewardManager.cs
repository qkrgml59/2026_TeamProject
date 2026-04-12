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
        public Canvas canvas;
        public RewardView[] views = new RewardView[3];

        private CardDataSO[] items = new CardDataSO[3];

        private List<CardDataSO>[] originPools = new List<CardDataSO>[3];

        public Action<CardDataSO> OnSelectReward;
        public Action OnSkip;

        public void Show(
            CardDataSO item_1, List<CardDataSO> pool_1,
            CardDataSO item_2, List<CardDataSO> pool_2,
            CardDataSO item_3, List<CardDataSO> pool_3)
        {
            if (item_1 != null) views[0].Show(item_1);
            if (item_2 != null) views[1].Show(item_2);
            if (item_3 != null) views[2].Show(item_3);

            originPools[0] = pool_1;
            originPools[1] = pool_2;
            originPools[2] = pool_3;


            canvas.enabled = true;
        }

        public void Select(int index)
        {
            if (index < 0 || index >= items.Length)
                return;

            if (items[index] == null)
                return;

            canvas.enabled = false;

            OnSelectReward?.Invoke(items[index]);
        }

        public void Skip()
        {
            for ( int i = 0; i< items.Length; i ++)
            {
                StageManager.Instance.ReturnCardToPool(items[i], originPools[i]);
            }

            ClearUI();

            OnSkip?.Invoke();
            canvas.enabled = false;
        }

        private void ClearUI()
        {
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = null;
                originPools[i] = null;
            }
        }

    }
}

