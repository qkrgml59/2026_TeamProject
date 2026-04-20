using Prototype.Grid;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Unit
{
    public enum UnitStateType
    {
        Null,
        Idle,
        Think,      // 경로나 적 탐색
        Move,
        Attack,
        Skill,
        Stun,
        Dead
    }

    public class IdleState : IUnitState
    {
        private UnitBase _unit;

        public IdleState(UnitBase unit)
        {
            _unit = unit;
        }

        public void StateEnter()
        {
            _unit.gameObject.SetActive(true);
            _unit.ClearPath();
        }

        public void StateUpdate()
        {
            
        }

        public void StateFixedUpdate()
        {
            
        }

        public void StateExit()
        {

        }

    }

    public class ThinkState : IUnitState
    {
        private UnitBase _unit;

        public ThinkState(UnitBase unit)
        {
            _unit = unit;
        }

        public void StateEnter()
        {
            // 스킬 사용 가능 상태인 경우
            if(_unit.CanUseSkill())
            {
                // 스킬 상태로 전환
                _unit.ChangeUnitState(UnitStateType.Skill);
                return;
            }

            // Think 상태 시작 시 빠른 판단
            if (_unit.targetUnit == null) return;

            // 사거리 안에 적이 있다면
            if(HexMath.Distance(_unit.offset,
                _unit.targetUnit.offset) <= _unit.statSet.AttackRange.Value)
            {
                // 공격 상태로 전환
                _unit.ChangeUnitState(UnitStateType.Attack);
                return;
            }

            // TODO : 이동 불가 상태를 따로 제작
            if (_unit.IsPathStillValid() && _unit.statSet.MoveSpeed.Value > 0)
            {
                _unit.ChangeUnitState(UnitStateType.Move);
                return;
            }
        }

        public void StateUpdate()
        {
            // 마나 재생
            _unit.RegenerateResource();

            // 대상이 없는 경우 가까운 적 탐색
            if (_unit.targetUnit == null)
            {
                var target = UnitManager.Instance.GetNearestEnemy(_unit);
                if (target == null)
                    return;

                _unit.SetTargetUnit(target);
            }


            // 스킬 사용이 가능하다면 사용
            if(_unit.CanUseSkill())
                _unit.ChangeUnitState(UnitStateType.Skill);

            // 사거리 안에 적이 있다면
            if (HexMath.Distance(_unit.offset,
                _unit.targetUnit.offset) <= _unit.statSet.AttackRange.Value)
            {
                // 공격 상태로 전환
                _unit.ChangeUnitState(UnitStateType.Attack);
                return;
            }

            // 경로가 없거나 기존 경로가 유효하지 않을 때
            if (!_unit.IsPathStillValid())
            {
                Debug.LogWarning("경로가 유효하지 않습니다.", _unit);
                // 경로 재탐색
                if (!_unit.TrySetPath())
                {
                    Debug.LogWarning("경로 못찾음", _unit);
                    return;
                }
            }

            // 경로가 있다면 Move상태로 전환
            _unit.ChangeUnitState(UnitStateType.Move);
        }

        public void StateFixedUpdate()
        {

        }

        public void StateExit()
        {

        }

    }

    public class MoveState : IUnitState
    {
        private UnitBase _unit;

        public MoveState(UnitBase unit)
        {
            _unit = unit;
        }

        public void StateEnter()
        {
            _unit.Kinematic(false);
            _unit.SetReservedTile();
        }

        public void StateUpdate()
        {
            // 마나 재생
            _unit.RegenerateResource();

            if (_unit.reservedTile == null)
            {
                _unit.SetReservedTile();
                return;
            }

            _unit.UpdatePathMovement();
        }

        public void StateFixedUpdate()
        {

        }

        public void StateExit()
        {
            // TODO: 물리 관련 설정 방식 변경 필요
            _unit.RigidInit();
            _unit.Kinematic(true);


            // 이동 종료 타이밍에 새로운 적 탐색
            var target = UnitManager.Instance.GetNearestEnemy(_unit);
            if (target == null)
                return;

            // 대상이 없거나 새로운 타겟이라면
            if (_unit.targetUnit == null || _unit.targetUnit != target) _unit.SetTargetUnit(target);
        }
    }

    public class AttackState : IUnitState
    {
        private UnitBase _unit;

        private float nextAttackTime = 0;

        public AttackState(UnitBase unit)
        {
            _unit = unit;
        }

        public void StateEnter()
        {
            _unit.ClearPath();
            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + 1f / _unit.statSet.AttackSpeed.Value;
                _unit.NormalAttack();
            }
        }

        public void StateUpdate()
        {
            // 마나 재생
            _unit.RegenerateResource();

            // 만약 Enter에서 일반 공격에 실패 했다면 Update에서 지속 체크

            // 대상이 없는 경우 생각 상태로 전환
            if (_unit.targetUnit == null)
            {
                _unit.ChangeUnitState(UnitStateType.Think);
                return;
            }

            // 스킬 사용이 가능하다면 사용
            if(_unit.CanUseSkill())
                _unit.ChangeUnitState(UnitStateType.Skill);

            // 사거리 안에 적이 없다면
            if (HexMath.Distance(_unit.offset,
                _unit.targetUnit.offset) > _unit.statSet.AttackRange.Value)
            {
                // 생각 상태로 전환
                _unit.ChangeUnitState(UnitStateType.Think);
                return;
            }

            // 공격 속도 체크
            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + 1f / _unit.statSet.AttackSpeed.Value;
                _unit.NormalAttack();
            }
        }

        public void StateFixedUpdate()
        {

        }

        public void StateExit()
        {
            if (_unit.normalAttack != null)
                _unit.normalAttack.CancelSkill();
        }
    }

    public class SkillState : IUnitState
    {
        private UnitBase _unit;

        public SkillState(UnitBase unit)
        {
            _unit = unit;
        }

        public void StateEnter()
        {
            _unit.UseSkill();
        }

        public void StateUpdate()
        {

        }

        public void StateFixedUpdate()
        {

        }

        public void StateExit()
        {
            _unit.skill.CancelSkill();
        }
    }

    public class StunState : IUnitState
    {
        private UnitBase _unit;

        public StunState(UnitBase unit)
        {
            _unit = unit;
        }

        public void StateEnter()
        {
            
        }

        public void StateUpdate()
        {
            // 마나 재생
            _unit.RegenerateResource();
        }

        public void StateFixedUpdate()
        {

        }

        public void StateExit()
        {

        }
    }

    public class DeadState : IUnitState
    {
        private UnitBase _unit;

        public DeadState(UnitBase unit)
        {
            _unit = unit;
        }

        public void StateEnter()
        {
            _unit.ClearPath();                          // 경로 제거
            _unit.unitEvents.OnDead?.Invoke(_unit);     // 사망 이벤트

            // TODO : 무적 & 부활 등 체크
            _unit.Die();
        }

        public void StateUpdate()
        {

        }

        public void StateFixedUpdate()
        {

        }

        public void StateExit()
        {
            
        }
    }


}
