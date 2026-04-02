using Unit.Skill;
using UnityEngine;

[CreateAssetMenu(fileName = "new UnitStat", menuName = "UnitStatSO/UnitStatSO")]
public class UnitStatSO : ScriptableObject
{
    [Header("기본 식별 정보")]
    public string ID;
    public string Name_KR;
    public string Name_EN;
    public RaceType Race;   //종족 (공포, 해적 ,등등)
    public int Cost;
    public Race_SynergyType Race_Synergy_A; //종족 고유 시너지 타입
    public Race_SynergyType Race_Synergy_B;
    public SynergyType Synergy_A;   //직업 시너지 타입
    public SynergyType Synergy_B;
    public SynergyType Synergy_C;

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
    [Header("최대마나")] public float MaxManaPoint;
    [Header("초기마나")] public float StartManaPoint;
    [Header("마나재생")] public float ManaRegeneration;


    [Header("행동 데이터")]
    [Header("일반공격 ID")] public string NormalAttack_Type;
    [Header("일반공격 프리팹")] public SkillBase NormalAttack_Prefab;
    [Header("스킬 ID")] public int Skill_ID;
    [Header("스킬 프리팹")] public SkillBase Skill_Prefab;

    // TODO : 임시 이미지, 추후 삭제
    [Header("(임시) 유닛 이미지")]
    public Texture2D unitSprite;
}

[System.Serializable]
public class StatByStar
{
    [Header("최대 체력")] public float MaxHp;
    [Header("공격력")] public float AttackDamage;
}
