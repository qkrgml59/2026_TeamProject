using Prototype.Grid;
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
        SKill,
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
            _unit.ClaerPath();
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
            if (_unit.IsSkillReady()
                && _unit.skill.CanUse())
            {
                // 스킬 상태로 전환
                _unit.ChangeUnitState(UnitStateType.SKill);
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


            if (_unit.IsPathStillValid())
            {
                _unit.ChangeUnitState(UnitStateType.Move);
                return;
            }
        }

        public void StateUpdate()
        {
            // 마나 재생
            _unit.RegenerateResource();

            // 스킬 사용 가능 상태인 경우
            if (_unit.IsSkillReady()
                && _unit.skill.CanUse())
            {
                // 스킬 상태로 전환
                _unit.ChangeUnitState(UnitStateType.SKill);
                return;
            }

            // 대상이 없는 경우 가까운 적 탐색
            if (_unit.targetUnit == null)
            {
                var target = _unit.GetNearestEnemy();
                if (target == null)
                    return;

                _unit.SetTargetUnit(target);
            }

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
        }
    }

    public class AttackState : IUnitState
    {
        private UnitBase _unit;


        public AttackState(UnitBase unit)
        {
            _unit = unit;
        }

        public void StateEnter()
        {
            _unit.ClaerPath();
            _unit.NormalAttack();
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
            if (_unit.skill != null)
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
            _unit.ClaerPath();                          // 경로 제거
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
