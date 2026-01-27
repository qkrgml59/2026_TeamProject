using System.Collections.Generic;

/// <summary>
/// 스텟 하나의 수치 단위
/// </summary>
public class Stat
{
    float baseValue;                                // 기초 스텟
    Dictionary<object, StatModifier> modifiers = new Dictionary<object, StatModifier>();     // 추가 스텟

    public Stat(float value = 0)
    {
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
        baseValue = value;
    }

    /// <summary>
    /// 스텟에 Modifier 추가
    /// (같은 source가 이미 존재하면 덮어쓴다)
    /// </summary>
    /// <param name="source">Modifier의 출처</param>
    /// <param name="modifier">적용할 스텟 변경 정보</param>
    public void AddModifier(object source, StatModifier modifier)
    {
        modifiers[source] = modifier;
    }

    /// <summary>
    /// 스텟 Modifier 제거
    /// </summary>
    /// <param name="source">제거 대상의 출처</param>
    public void RemoveModifier(object source)
    {
        if (modifiers.ContainsKey(source))
            modifiers.Remove(source);
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
