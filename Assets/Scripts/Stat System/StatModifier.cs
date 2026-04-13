using UnityEngine;

namespace StatSystem
{

    /// <summary>
    /// 추가 스텟을 관리하는 클래스
    /// </summary>
    [System.Serializable]
    public struct StatModifier
    {
        [Header("대상 스텟")] public StatType statType;                                          // 스텟 종류
        [Header("연산 방식 ( 고정 수치 증가 or % 증가 )")] public ModifierType modifierType;      // 강화 타입
        [Header("수치 ( 10% 는 10으로 입력 )")] public float value;                              // 강화량


        /// <summary>
        /// 두 Modifier가 서로 같은 값을 가지고 있는지 확인
        /// </summary>
        public bool Equals(StatModifier modifier)
        {
            if(statType != modifier.statType) return false;
            if(modifierType != modifier.modifierType) return false;
            if(value != modifier.value) return false;

            return true;
        }
    }
}
