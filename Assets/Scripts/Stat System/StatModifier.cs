using UnityEngine;

/// <summary>
/// 스텟 강화 효과를 관리하는 클래스
/// </summary>
[System.Serializable]
public class StatModifier
{
    [Header("대상 스텟")] public StatType statType;                                          // 스텟 종류
    [Header("연산 방식 ( 고정 수치 증가 or % 증가 )")]public ModifierType modifierType;      // 강화 타입
    [Header("수치 ( 10% 는 10으로 입력 )")] public float value;                              // 강화량
}
