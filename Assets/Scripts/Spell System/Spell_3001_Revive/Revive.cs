using Prototype.Grid;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Spell
{
    [CreateAssetMenu(fileName = "ReviveSpellSO", menuName = "SpellSO/Effects/ReviveSpell")]
    public class Revive : SpellEffectSO
    {
        public override bool TryExecute(HexTile tile)
        {
            return true;
        }

        public override string GetDescription()
        {
            return "";
        }
    }
}
