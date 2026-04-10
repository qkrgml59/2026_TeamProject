using Unit;
using UnityEngine;

namespace Item
{
    [CreateAssetMenu(fileName = "LowHpHealEffect", menuName = "ItemSO/Effects/Low HP Heal")]
    public class LowHpHealEffectSO : ItemEffectSO
    {
        [Header("발동 조건 및 수치")]
        [Tooltip("발동 기준 체력 퍼센트 0.1당 = 10%")] public float triggerHpPercent = 0.1f;
        [Tooltip("회복할 최대 체력 비례 퍼센트 0.1 = 10%")] public float healPercent = 0.1f;

        private UnitBase _equippedUnit;
        private bool _hasTriggered;

        public override void OnEquip(UnitBase unit)
        {
            _equippedUnit = unit;
            _hasTriggered = false;

            _equippedUnit.unitEvents.OnHpChanged.AddListener(CheckEffect);
            BattleManager.Instance.OnBattleStart += ResetEffect;
        }

        public override void OnUnequip(UnitBase unit)
        {
            if (_equippedUnit != null)
                _equippedUnit.unitEvents.OnHpChanged.RemoveListener(CheckEffect);

            if (BattleManager.Instance != null)
                BattleManager.Instance.OnBattleStart -= ResetEffect;

            _equippedUnit = null;
        }

        private void ResetEffect() => _hasTriggered = false;

        private void CheckEffect(float currentHp, float maxHp)
        {
            if (BattleManager.Instance.currentBattleState != BattleState.Combat) return;
            if (_hasTriggered) return;
            if (_equippedUnit == null || _equippedUnit.CurFSM == UnitStateType.Dead) return;

            // 설정한 퍼센트 이하로 체력이 떨어졌는지 검사
            if (currentHp / maxHp <= triggerHpPercent)
            {
                _hasTriggered = true;

                float healAmount = maxHp * healPercent;
                HealInfo healInfo = new HealInfo(_equippedUnit, healAmount);
                _equippedUnit.ApplyHeal(healInfo);

                Debug.Log($"{_equippedUnit.name} 체력 {triggerHpPercent * 100}% 도달 {healAmount} 회복");
            }
        }
    }
}
