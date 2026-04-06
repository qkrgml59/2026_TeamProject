using Unit.Skill;
using UnityEngine;


namespace Unit
{
    [CreateAssetMenu(fileName = "new Stat", menuName = "UnitSO/Stat SO")]
    public class UnitStatSO : ScriptableObject
    {
        [Header("기물 성급에 따른 기초 스텟")]
        public StatByStar[] statsByStart = new StatByStar[3];

        [Header("기본 스텟 (성급 무관 고정값)")]
        [Header("방어력")] public float Defense;
        [Header("마법 저항력")] public float MagicResistance;
        [Header("주문력 (기본 0)")] public float AbilityPower;
        [Header("(초당) 공격 속도")] public float AttackSpeed;

        [Header("특수 스텟")]
        [Header("기본공격사거리")] public int Attack_Rage = 1;
        [Header("이동속도")] public float Move_Speed = 1.0f;
        //[Header("보조 스텟")]public string unitType = "전사/암살자 같은 분류 예정"; //enum으로 바꾸면 어떨까 싶은 생각

        [Header("자원 스텟")]
        [Header("마나재생")] public float ManaRegeneration;
    }

    [System.Serializable]
    public class StatByStar
    {
        [Header("최대 체력")] public float MaxHp;
        [Header("공격력")] public float AttackDamage;
    }
}
