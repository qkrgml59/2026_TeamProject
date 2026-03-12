using Prototype.Unit;
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
            if(_caster == null || _targetUnit == null) return;

            if(_targetUnit.CurFSM == UnitStateType.Dead)
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
                _targetUnit?.ApplyDamage(_caster.statSet.AttackDamage.Value);
                Destroy(gameObject);
            }
        }
    }
}
