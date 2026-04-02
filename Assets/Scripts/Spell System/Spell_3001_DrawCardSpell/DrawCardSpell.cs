using Prototype.Card;
using Prototype.Grid;
using UnityEngine;

namespace Spell.Effects
{
    [CreateAssetMenu(fileName = "DrawCardSpell", menuName = "SpellSO/Effects/DrawCardSpell")]
    public class DrawCardSpell:SpellEffectSO
    {
        [Header("뽑을 카드의 개수")] public int drawCount = 1;

        public override bool TryExecute(RaycastHit hit)
        {
            HexTile tile = hit.transform.GetComponent<HexTile>();

            if (tile != null)
            {
                BattleCardManager.Instance.DrawCard();

                Debug.Log($"카드를 {drawCount}장 뽑습니다.");

                return true;
            }

            Debug.LogWarning("타일 위에 카드를 사용하세요!");
            return false;
        }

        public override string GetDescription() => $"카드를 {drawCount}장 뽑습니다.";
    }
}
