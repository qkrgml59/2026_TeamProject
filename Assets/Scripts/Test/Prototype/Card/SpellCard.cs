using Prototype.Card;
using Prototype.Card.Spell;
using Prototype.Grid;
using Spell;
using UnityEngine;

namespace Prototype.Card
{
    public class SpellCard : CardBase
    {
        SpellCardDataSO spell;

        public override void Init(CardDataSO data)
        {
            base.Init(data);
            spell = data as SpellCardDataSO;

            if (spell == null)
            {
                Debug.LogError($"{data.cardName}이 SpellCardDataSO 타입이 아님.");
            }
        }

        protected override bool TryUseCard(RaycastHit hit)
        {
            if (spell == null || spell.spellSO  == null) return false;

            SpellSO spellData = spell.spellSO;

            if (spellData.effect != null)
            {
                HexTile tile = hit.transform.GetComponent<HexTile>();

                if (tile != null)
                {
                    bool isSuccess = spellData.effect.TryExecute(tile);
                    return isSuccess;
                }

                Debug.Log("타일 위에 사용하십시오");
            }

            return false;
        }
    }
}
