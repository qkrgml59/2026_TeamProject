using Prototype.Card;
using Prototype.Grid;
using System.Collections;
using UnityEngine;

namespace Spell.Effects
{
    [CreateAssetMenu(fileName = "DrawCardSpell", menuName = "SpellSO/Effects/DrawCardSpell")]
    public class DrawCardSpell:SpellEffectSO
    {
        [Header("뽑을 카드의 개수")] public int drawCount = 1;

        public override bool TryExecute(HexTile tile)
        {
            BattleCardManager.Instance.StartCoroutine(DrawCardCoroutines());

            Debug.Log($"카드를 {drawCount}장 뽑습니다.");

            return true;
        }

        private IEnumerator DrawCardCoroutines()
        {
            yield return new WaitForEndOfFrame();

            BattleCardManager.Instance.DrawCard(drawCount);
        }

        public override string GetDescription() => $"카드를 {drawCount}장 뽑습니다.";
    }
}
