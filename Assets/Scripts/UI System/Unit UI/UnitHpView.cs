using StatSystem;
using Unit;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UnitHpView : MonoBehaviour
    {
        [Header("현재 체력 표시")]
        public Image curHp;
        RectTransform curHpRect;

        [Header("보호막 표시")]
        public Image curShield;
        RectTransform curShieldRect;

        [Header("체력바 색상")]
        public Color allyColor;
        public Color enemyColor;

        // UI 컴포넌트
        RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            if (curHp != null) curHpRect = curHp.GetComponent<RectTransform>();
            if (curShield != null) curShieldRect = curShield.GetComponent<RectTransform>();
        }

        public void SetTeamColor(TeamType team)
        {
            if (curHp == null) return;

            if (team == TeamType.Ally) curHp.color = allyColor;
            else if(team == TeamType.Enemy) curHp.color = enemyColor;
        }

        public void OnHpChanged(HealthInfo info)
        {
            if (curHp == null) return;

            float hpRatio = info.HpRatio;
            curHp.fillAmount = hpRatio;

            if (curShield == null) return;

            if (curHpRect != null && curShieldRect != null)
            {
                // Hp바가 끝나는 지점부터 시작
                curShield.fillAmount = info.ShieldRatio;

                curShieldRect.anchoredPosition = new Vector2(curHpRect.anchoredPosition.x + curHpRect.sizeDelta.x * hpRatio, 0);
            }
            else
            {
                // Hp바의 크기를 구하지 못한 경우, Hp 뒤로 쉴드도 채움
                curShield.fillAmount = hpRatio + info.ShieldRatio;
            }
        }

    }
}
