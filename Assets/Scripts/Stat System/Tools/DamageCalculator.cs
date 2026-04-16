using Unit;
using UnityEngine;

namespace StatSystem
{

    public static class DamageCalculator
    {
        /// <summary>
        /// 방어력 및 피해 감소를 계산한 최종 피해량을 반환 
        /// </summary>
        public static float CalculateFinalDamage(DamageInfo info, UnitBase hitUnit)
        {
            StatSet statset = hitUnit.statSet;

            float damage = info.amount;

            float multiplier = 1f;

            switch (info.damageType)
            {
                case DamageType.Physical:
                multiplier = CalculateDefenseMultiplier(statset.Defense.Value);
                    break;
                case DamageType.Magical:
                    multiplier = CalculateDefenseMultiplier(statset.MagicResist.Value);
                    break;
                case DamageType.True:
                    multiplier = 1f;
                    break;
                default:
                    Debug.LogWarning($"[{nameof(DamageCalculator)}] {info.caster.name}의 공격의 종류가 지정 되지 않았습니다.", info.caster);
                    break;
            }

            damage *= multiplier;

            // 피해 감소 배율 계산
            damage *= (100 - statset.DamageReduction.Value) * 0.01f;

            return damage;
        }

        /// <summary>
        /// 방어력 및 마법 저항력에 따른 최종 피해 배율을 반환
        /// </summary>
        public static float CalculateDefenseMultiplier(float defense)
        {
            if (defense >= 0)
                return 1 / (1 + defense * 0.01f);
            else
                return 2 - (1 / (1 - defense * 0.01f));         // 방어력이 음수 일 때는 다른 공식 사용
        }

        /// <summary>
        /// 치명타 발생 여부 확인
        /// </summary>
        /// <param name="criticalChance"></param>
        /// <returns></returns>
        public static bool RollCritical(float criticalChance)
        {
            // TODO: 시드값을 통한 확률 계산으로 변경 필요
            float rand = Random.value;

            return (rand < criticalChance * 0.01f);
        }
    }
}
