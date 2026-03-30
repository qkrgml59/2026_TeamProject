using Spell;
using UnityEngine;

namespace Prototype.Card.Spell
{
    [CreateAssetMenu(fileName = "CardDataSO", menuName = "CardDataSO/SpellDataSO")]
    public class SpellCardDataSO : CardDataSO
    {
        public SpellSO spellSO;
    }
}
