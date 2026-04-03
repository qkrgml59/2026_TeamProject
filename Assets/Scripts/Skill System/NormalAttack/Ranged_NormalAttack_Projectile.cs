using Stat;
using UnityEngine;

namespace Unit.Skill
{
    public class Ranged_NormalAttack_Projectile : MonoBehaviour
    {
        public float moveSpeed = 10f;

        DamageInfo _info;
        UnitBase _targetUnit;

        private Vector3 targetLastPos;
        public void Initialize(DamageInfo info, UnitBase targetUnit)
        {
            _info = info;
            _targetUnit = targetUnit;
            targetLastPos = targetUnit.transform.position;
        }

        private void Update()
        {
            if(_targetUnit == null || _targetUnit.CurFSM == UnitStateType.Dead)
            {
                // 타겟이 죽은 경우, 최종 위치로 발사
                Movement(targetLastPos);
                return;
            }

            targetLastPos = _targetUnit.transform.position;
            Movement(_targetUnit.transform.position);
        }

        private void Movement(Vector3 targetPos)
        {
            Vector3 currentPos = transform.position;
            Vector3 nextPos = Vector3.MoveTowards(currentPos, targetPos, moveSpeed * Time.deltaTime);

            Vector3 dir = (targetPos - currentPos);
            if (dir.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(dir.normalized);
            }

            transform.position = nextPos;

            if ((targetPos - transform.position).sqrMagnitude < 0.01f)
            {
                TakeDamage();
            }
        }

        void TakeDamage()
        {
            if (_targetUnit != null)
            {
                _targetUnit.ApplyDamage(_info);

                // caster의 일반 공격 적중 이벤트 호출
                _info.caster.unitEvents.OnNormalAttackHit?.Invoke(_info, _targetUnit);
            }

            // 공격 후 투사체 파괴
            Destroy(gameObject);
        }
    }
}
