/// <summary>
/// 스텟의 종류
/// </summary>
public enum StatType
{
    HealthPoint = 0,            // 체력
    AttackDamage = 1,           // 공격력
    AbilityPower = 2,           // 주문력
    AttackSpeed = 3,            // 공격 속도
    Defense = 4,                // 방어력
    MagicResistance = 5,        // 마법 저항력
    ManaRegeneration = 6,       // 마나 재생
    CriticalChance = 7,         // 치명타 확률
    CriticalDamage = 8,         // 치명타 피해량
    DamageIncrease = 9,         // 가하는 피해 증가
    DamageReduction = 10,       // 받는 피해 감소
    Omnivamp = 11,              // 생명력 흡수
    AttackRange = 12,           // 사거리
    MoveSpeed = 13,             // 이동 속도
    MaxMana = 14,               // 최대 마나
    StartMana = 15,             // 초기 마나
}
