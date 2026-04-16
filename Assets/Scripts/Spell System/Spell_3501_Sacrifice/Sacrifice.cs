using Prototype.Card;
using Prototype.Grid;
using UnityEngine;

namespace Spell
{
    [CreateAssetMenu(fileName = "SacrificeSpell", menuName = "SpellSO/Effects/SacrificeSpell")]

    public class Sacrifice : SpellEffectSO
    {
        [Header("Spell Setting")]
        [Tooltip("버릴 카드의 개수")] public int discardCardAmount = 1;

        [Tooltip("회복 할 코스트")] public int recoverCostAmount = 2;

        public override bool TryExecute(HexTile tile)
        {
            bool isDiscarded = BattleCardManager.Instance.DiscardRandomCard(discardCardAmount);

            if (!isDiscarded)           // 손패의 카드가 버릴 개수보다 적을 때 사용 실패
            {
                Debug.LogWarning($"손패에 카드가 ({discardCardAmount}장)이 부족하여 사용 실패.");
                return false;
            }

            CostManager.Instance.RecoverCost(recoverCostAmount);
            Debug.Log($"손패에서 {BattleCardManager.Instance.currentCastingCard.cardName}을 버린 후, Cost{recoverCostAmount}회복");

            return true;
        }


        public override string GetDescription()
        {
            return $"손패에서 무작위 {discardCardAmount}장을 버린 후, 플레이어의 Cost{recoverCostAmount}만큼 회복";
        }
    }
}
