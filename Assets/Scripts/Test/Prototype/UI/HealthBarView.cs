using UnityEngine;
using UnityEngine.UI;
using Prototype.Unit;

namespace Prototype.UI
{
    public class HealthBarView : MonoBehaviour
    {
        public Image currentHPBar;
        public RectTransform rectTransform;
        UnitBase _Unit;

        public void Init(UnitBase unit)
        {
            _Unit = unit;

            // 포지션 초기화
            UpdatePosition();

            // 체력 초기화
            float maxHp = _Unit.statSet.MaxHp.Value;
            UpdateHp(maxHp, maxHp);

            // 이벤트 등록
            _Unit.unitEvents.OnHpChanged.AddListener(UpdateHp);
        }

        private void Update()
        {
            if (_Unit != null && _Unit.CurFSM != UnitStateType.Dead)
                UpdatePosition();
        }

        public void UpdatePosition()
        {
            Vector3 worldPos = _Unit.transform.position;
            worldPos += Vector3.up * 4f;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            rectTransform.position = screenPos;
        }

        public void UpdateHp(float current, float max)
        {
            if (max == 0)
            {
                Debug.LogError("최대 체력은 0이 될 수 없습니다.");
                return;
            }

            if(currentHPBar == null)
            {
                Debug.LogWarning("currentHPBar가 없습니다.", this);
                return;
            }

            currentHPBar.fillAmount = current / max;
        }

        private void OnDisable()
        {
            _Unit.unitEvents.OnHpChanged.RemoveListener(UpdateHp);
        }
    }
}
