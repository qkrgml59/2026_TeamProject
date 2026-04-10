using System.Collections.Generic;
using System;

namespace Stat
{
    /// <summary>
    /// 스텟 하나의 수치 단위
    /// </summary>
    public class Stat
    {
        /// <summary>
        /// 스텟의 값이 변할때 호출됩니다. (baseValue 변경, modifier 추가 등)
        /// </summary>
        public event Action<Stat> onValueChanged;

        public StatType statType {get; private set;}

        float baseValue;                                // 기초 스텟
        public float BaseValue => baseValue;
        Dictionary<object, StatModifier> modifiers = new Dictionary<object, StatModifier>();     // 추가 스텟

        public Stat(StatType statType, float value = 0)
        {
            this.statType = statType;

            // 기초 스텟 적용
            SetBaseValue(value);

            // 추가 스텟 초기화
            modifiers = new Dictionary<object, StatModifier>();
        }

        /// <summary>
        /// 모든 Modifier를 적용한 최종 스텟 값
        /// </summary>
        public float Value => CalculateFinalValue();    // 최종 스텟

        /// <summary>
        /// 기초 스텟 설정
        /// </summary>
        public void SetBaseValue(float value)
        {
            if(baseValue == value) return;

            baseValue = value;
            onValueChanged?.Invoke(this);        // Value 변경 이벤트
        }

        /// <summary>
        /// 스텟에 Modifier 추가
        /// (같은 source가 이미 존재하면 덮어쓴다)
        /// </summary>
        /// <param name="source">Modifier의 출처</param>
        /// <param name="modifier">적용할 스텟 변경 정보</param>
        public void AddModifier(object source, StatModifier modifier)
        {
            // 맞지 않는 스텟을 추가할 경우 무시
            if(statType != modifier.statType) return;

            // 같은 값을 중복해서 추가 하는 경우 무시
            if(modifiers.ContainsKey(source) && modifiers[source].Equals(modifier)) return;
            

            modifiers[source] = modifier;
            onValueChanged?.Invoke(this);        // Value 변경 이벤트
        }

        /// <summary>
        /// 스텟 Modifier 제거
        /// </summary>
        /// <param name="source">제거 대상의 출처</param>
        public void RemoveModifier(object source)
        {
            if (!modifiers.ContainsKey(source)) return;

            modifiers.Remove(source);
            onValueChanged?.Invoke(this);        // Value 변경 이벤트
        }


        /// <summary>
        /// 적용 되어 있는 modifier를 모두 제거합니다.
        /// </summary>
        public void ClaerModifier()
        {
            modifiers.Clear();
            onValueChanged?.Invoke(this);        // Value 변경 이벤트
        }

        /// <summary>
        /// 해당 스텟의 정보를 초기화 합니다.
        /// </summary>
        public void Reset()
        {
            baseValue = 0;
            ClaerModifier();
        }

        // 추가 스텟(modifiers)를 포함한 최종 스텟을 반환
        float CalculateFinalValue()
        {
            float finalValue = baseValue;

            float flatSum = 0f;                 // 고정 수치 추가량
            float percentSum = 0f;              // %증가량

            foreach (var mod in modifiers.Values)
            {
                if (mod.modifierType == ModifierType.Flat)
                    flatSum += mod.value;
                else if (mod.modifierType == ModifierType.Percent)
                    percentSum += mod.value * 0.01f;                    // 백분율에서 변환
            }


            // 고정 수치 증가 후 %증가 적용
            finalValue += flatSum;
            finalValue *= (1f + percentSum);

            return finalValue;
        }
    }
}
