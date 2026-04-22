using StatSystem;
using TMPro;
using Unit;
using Unit.Skill;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    // 스킬 표시를 위해 임의로 만든 컨트롤러 + View 입니다.
    // TODO : 유닛 스텟이나 성급 맞춰서 보이도록 수정
    public class SkillViewController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // 아이콘 컴포넌트

        // 이름 텍스트 컴포넌트

        [Header("디테일 패널")]
        public RectTransform skillDetailPanel;

        public TextMeshProUGUI descriptionTMP;

        // 정보 저장
        private SkillBase skill;
        private StatSet unitStatSet;


        public void Init(SkillBase skill, StatSet unitStatSet)
        {
            if (skill == null || unitStatSet == null) return;

            this.skill = skill;
            this.unitStatSet = unitStatSet;
        }

        public void Clear()
        {
            skill = null;
            unitStatSet = null;
            if (descriptionTMP != null) descriptionTMP.text = "Skill Description";
        }

        void UpdateDetail()
        {
            if (skill == null || unitStatSet == null) return;

            if (descriptionTMP != null) descriptionTMP.text = skill.GetDescription(unitStatSet);

            if(skillDetailPanel != null) LayoutRebuilder.ForceRebuildLayoutImmediate(skillDetailPanel);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            UpdateDetail();

            if (skillDetailPanel != null) skillDetailPanel.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (skillDetailPanel != null) skillDetailPanel.gameObject.SetActive(false);
        }
    }
}
