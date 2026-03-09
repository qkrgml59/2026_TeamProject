using Prototype.Grid;
using UnityEngine;

namespace Prototype.Unit
{
    public enum UnitStateType
    {
        Null,
        Idle,
        Think,      // 경로나 적 탐색
        Move,
        Attack,
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
            // Think 상태 시작 시 빠른 판단
            if (_unit.targetUnit == null) return;

            // 사거리 안에 적이 있다면
            if(HexMath.Distance(_unit.currentTile.offset,
                _unit.targetUnit.currentTile.offset) <= _unit.statSet.AttackRange.Value)
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
            // 대상이 없는 경우 가까운 적 탐색
            if (_unit.targetUnit == null)
            {
                var target = _unit.GetNearestEnemy();
                if (target == null)
                    return;

                _unit.SetTargetUnit(target);
            }

            // 사거리 안에 적이 있다면
            if (HexMath.Distance(_unit.currentTile.offset,
                _unit.targetUnit.currentTile.offset) <= _unit.statSet.AttackRange.Value)
            {
                // 공격 상태로 전환
                _unit.ChangeUnitState(UnitStateType.Attack);
                return;
            }

            // 경로가 없거나 기존 경로가 유효하지 않을 때
            if (!_unit.IsPathStillValid())
            {
                Debug.LogWarning("경로 없음");
                // 경로 재탐색
                if (!_unit.TrySetPath())
                {
                    Debug.LogWarning("경로 못찾음");
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
        }

        public void StateUpdate()
        {
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

        private float interval = 0;

        public AttackState(UnitBase unit)
        {
            _unit = unit;
        }

        public void StateEnter()
        {
            interval = 0;
        }

        public void StateUpdate()
        {
            interval += Time.deltaTime;
            if(interval >= _unit.statSet.AttackSpeed.Value)
            {
                Debug.Log($"[{_unit.name}] 공격!");
                interval = 0;
            }
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
            _unit.gameObject.SetActive(false);
            _unit.ClaerPath();
            _unit.EnterTile(null);      // 위치했던 타일 제거
        }

        public void StateUpdate()
        {

        }

        public void StateFixedUpdate()
        {

        }

        public void StateExit()
        {
            _unit.gameObject.SetActive(true);
        }
    }


}
