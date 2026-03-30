using UnityEngine;

namespace Spell
{
    [CreateAssetMenu(fileName = "DefaultSpell", menuName = "SpellSO/Default")]
    public class SpellSO : ScriptableObject
    {
        [Header("스펠 아이콘")] public Sprite icon;
        [Header("스펠 이름")] public string spellName;
        [Header("스펠 설명")] [TextArea] public string description;

        [Header("스펠 발동 효과")] public SpellEffectSO effect;

        public string GetEffectDescription()
        {
            if (effect == null)
                return null;

            return effect.GetDescription();
        }
    }
}
