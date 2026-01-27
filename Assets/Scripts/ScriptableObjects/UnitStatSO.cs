using UnityEngine;

[CreateAssetMenu(fileName = "new UnitStat", menuName = "UnitStatSO/UnitStatSO")]
public class UnitStatSO : ScriptableObject
{
    [Header("기물 성급에 따른 기초 스텟")]
    public StatByStar[] statsByStart = new StatByStar[3];

    // 특수 스텟
    [Header("사거리")]
    public int Attack_Rage = 1;
    [Header("이동 속도")]
    public float Move_Speed = 1.0f;
    [Header("보조 스텟")]
    public string unitType = "전사/암살자 같은 분류 예정";
}

[System.Serializable]
public class StatByStar
{
    [Header("최대 체력")] public float MaxHp;
    [Header("공격력")] public float AttackDamage;
    [Header("주문력 (기본 0)")] public float AbilityPower;
    [Header("공격 속도")] public float AttackSpeed;
    [Header("방어력")] public float Defense;
    [Header("마법 저항력")] public float MagicResistance;
}
