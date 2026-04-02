using Prototype.Card;
using UnityEngine;
using Utilitys;

namespace Test.UI
{

    public class TestRewardManager : SingletonMonoBehaviour<TestRewardManager>
    {
        public Canvas canvas;

        public TestRewardView[] views = new TestRewardView[3];

        private CardDataSO[] items = new CardDataSO[3];

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
            // TODO : 카드 풀에서도 제거 하는 기능 추가
            DeckManager.Instance.TryAddCardToDeck(items[index]);
            BattleManager.Instance.NextRound();
            canvas.enabled = false;
        }

        public void Skip()
        {
            // 스킵
            BattleManager.Instance.NextRound();
            canvas.enabled = false;
        }
    }
}
