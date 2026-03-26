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
        }

        private void Update()
        {
            if(_targetUnit == null || _targetUnit.CurFSM == UnitStateType.Dead)
            {
                // 타겟이 죽은 경우, 최종 위치로 발사
                Movement(targetLastPos);
                return;
            }

            Movement(_targetUnit.transform.position);
        }

        private void Movement(Vector3 targetPos)
        {
            Vector3 dir = (targetPos - transform.position).normalized;

            transform.position += dir * moveSpeed * Time.deltaTime;

            transform.rotation = Quaternion.LookRotation(dir);

            if (Vector3.Distance(targetPos, transform.position) < 0.1f)
            {
                TakeDamage();
            }
        }

        void TakeDamage()
        {
            _targetUnit?.ApplyDamage(_info);

            // 공격 후 투사체 파괴
            Destroy(gameObject);
        }
    }
}
