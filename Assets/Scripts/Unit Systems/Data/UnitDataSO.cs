using Unit.Skill;
using UnityEngine;

namespace Unit
{

    [CreateAssetMenu(fileName = "new Unit", menuName = "UnitSO/Unit SO")]
    public class UnitDataSO : ScriptableObject
    {
        [Header("기본 식별 정보")]
        public string ID;
        public string Name_KR;
        public string Name_EN;
        public ThemeType Race;   //종족 (공포, 해적 ,등등)
        public int Cost;
        public Race_SynergyType Race_Synergy_A; //종족 고유 시너지 타입
        public Race_SynergyType Race_Synergy_B;
        public SynergyType Synergy_A;   //직업 시너지 타입
        public SynergyType Synergy_B;
        public SynergyType Synergy_C;

        [Header("스텟 정보")]
        public UnitStatSO statData;     // 스텟 정보

        [Header("행동 데이터")]
        [Header("일반공격 ID")] public string NormalAttack_Type;
        [Header("일반공격 프리팹")] public SkillBase NormalAttack_Prefab;
        [Header("스킬 ID")] public int Skill_ID;
        [Header("스킬 프리팹")] public SkillBase Skill_Prefab;

        // TODO : 임시 이미지, 추후 삭제
        [Header("(임시) 유닛 이미지")]
        public Texture2D unitSprite;
    }
}
