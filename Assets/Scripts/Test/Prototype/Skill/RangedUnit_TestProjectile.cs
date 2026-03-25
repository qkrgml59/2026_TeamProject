using Prototype.Unit;
using Stat;
using UnityEngine;

namespace Prototype.Skill
{
    public class RangedUnit_TestProjectile : MonoBehaviour
    {
        public float moveSpeed = 10f;

        UnitBase _caster;
        UnitBase _targetUnit;
        public void Initialize(UnitBase caster, UnitBase targetUnit)
        {
            _caster = caster;
            _targetUnit = targetUnit;
        }

        private void Update()
        {
            if(_caster == null || _targetUnit == null || _targetUnit.CurFSM == UnitStateType.Dead)
            {
                // 일단 파괴
                Destroy(gameObject);
                return;
            }

            Movement();
        }

        private void Movement()
        {
            Vector3 dir = (_targetUnit.transform.position - transform.position).normalized;

            transform.position += dir * moveSpeed * Time.deltaTime;

            transform.rotation = Quaternion.LookRotation(dir);

            if(Vector3.Distance(_targetUnit.transform.position, transform.position) < 0.1f)
            {
                TakeDamage();
            }
        }

        void TakeDamage()
        {
            // 일반 공격은 공격력의 100% 데미지
            float damage = _caster.statSet.AttackDamage.Value;

            // 치명타 확인
            bool isCrit = DamageCalculator.RollCritical(_caster.statSet.CritChance.Value);
            if (isCrit)
            {
                // 치명타 발생 시
                damage *= (_caster.statSet.CritDamage.Value * 0.01f);
            }


            // 피해 증가 배율 증가
            damage *= (100 + _caster.statSet.DamageIncrease.Value) * 0.01f;

            DamageInfo info = new DamageInfo(_caster, damage, DamageType.Physical, isCrit);

            _targetUnit?.ApplyDamage(info);

            // 공격 후 투사체 파괴
            Destroy(gameObject);
        }
    }
}
