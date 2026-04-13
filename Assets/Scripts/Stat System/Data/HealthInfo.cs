using UnityEngine;

namespace StatSystem
{
    /// <summary>
    /// 유닛의 체력 관련 정보를 전달하는 구조체
    /// </summary>
    public struct HealthInfo
    {
        public float currentHp;
        public float maxHp;
        public float shield;

        public HealthInfo(float currentHp, float maxHp, float shield)
        {
            this.currentHp = currentHp;
            this.maxHp = maxHp;
            this.shield = shield;
        }

        /// <summary>
        /// 총 체력 기준(체력 + 보호막) 현재 체력의 비율을 반환합니다.
        /// </summary>
        public float HpRatio => maxHp + shield > 0 ?(float)currentHp / (maxHp + shield) : 0;                   // 체력 합이 0 이하라면 0으로
        /// <summary>
        /// 총 체력 기준(체력 + 보호막) 보호막의 비율을 반환합니다.
        /// </summary>
        public float ShieldRatio => maxHp + shield > 0 ? (float)shield / (maxHp + shield) : 0;
    }
}
