using Prototype.Card;
using System;
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

        public Action<CardDataSO> OnSelectReward;
        public Action OnSkip;

        public void Show(CardDataSO item_1, CardDataSO item_2, CardDataSO item_3)
        {
            views[0].Show(item_1);
            views[1].Show(item_2);
            views[2].Show(item_3);

            items[0] = item_1;
            items[1] = item_2;
            items[2] = item_3;

            canvas.enabled = true;
        }

        public void Select(int index)
        {
            canvas.enabled = false;

            OnSelectReward?.Invoke(items[index]);
        }

        public void Skip()
        {
            canvas.enabled = false;

            OnSkip?.Invoke();
        }

    }
}

