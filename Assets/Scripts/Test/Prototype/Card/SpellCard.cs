using Prototype.Card;
using Prototype.Card.Spell;
using Prototype.Grid;
using Spell;
using Unit;
using UnityEngine;

namespace Prototype.Card
{
    public class SpellCard : CardBase
    {
        SpellCardDataSO spell;

        public override TeamType GetTargetTile()
        {
            // 스펠은 스펠에 따라 사용 타일이 다름
            if(spell != null && spell.spellSO != null)
            {
                return spell.spellSO.tileArea;
            }

            return TeamType.Null;
        }

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

                if (tile == null)
                {
                    Debug.Log("타일 위에 사용하십시오");
                    return false;
                }

                if(tile.TeamType != GetTargetTile() && GetTargetTile() != TeamType.Both)
                {
                    Debug.Log("사용 가능한 타일이 아닙니다.");
                    return false;
                }

                bool isSuccess = spellData.effect.TryExecute(tile);
                return isSuccess;
            }

            return false;
        }
    }
}
