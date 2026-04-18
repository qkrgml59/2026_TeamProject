using StatSystem;
using UnityEngine;

namespace StatSystem
{

    public static class StatFormatter
    {
        public static string StatToString(float value, StatType statType)
        {
            string text = "";
            switch (statType)
            {
                // 정수값 표시
                case StatType.HealthPoint:
                case StatType.AttackDamage:
                case StatType.Defense:
                case StatType.MagicResistance:
                case StatType.ManaRegeneration:
                    text = $"{value.ToString("F0")}";
                    break;

                // % 표시
                case StatType.AbilityPower:
                case StatType.CriticalChance:
                case StatType.CriticalDamage:
                case StatType.DamageIncrease:
                case StatType.DamageReduction:
                case StatType.Omnivamp:
                    text = $"{value.ToString("F0")}%";
                    break;

                // 소수점 포함 표시
                case StatType.AttackSpeed:
                    text = $"{value.ToString("F2")}";
                    break;

                default:
                    text = "None";
                    break;
            }

            return text;
        }

        public static string ModifierToString(StatModifier modifier)
        {
            string text = $"<sprite name=\"{modifier.statType}\"> ";

            text += modifier.value.ToString("+0;-0;0");               // 양수는 + / 음수는 - 부호 붙여주고, 소수점 반올림

            switch (modifier.modifierType)
            {
                case ModifierType.Percent:
                    text += "%";
                    break;
                default:
                    // 아무런 효과 없음
                    break;
            }

            return text;
        }
    }
}
