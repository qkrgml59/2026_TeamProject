using Spell;
using UnityEngine;

namespace Prototype.Card.Spell
{
    [CreateAssetMenu(fileName = "Card_Spell", menuName = "CardDataSO/SpellCardDataSO")]
    public class SpellCardDataSO : CardDataSO
    {
        public SpellSO spellSO;
        public override CardType CardType => CardType.Spell;
    }
}
