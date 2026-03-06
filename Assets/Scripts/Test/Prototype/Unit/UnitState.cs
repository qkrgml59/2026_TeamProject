using UnityEngine;

namespace Prototype.Unit
{
    public enum UnitStateType
    {
        Null,
        Idle,
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
        }

        public void StateExit()
        {
            
        }

        public void StateFixedUpdate()
        {
            
        }

        public void StateUpdate()
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

        }

        public void StateExit()
        {

        }

        public void StateFixedUpdate()
        {

        }

        public void StateUpdate()
        {

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

        }

        public void StateExit()
        {

        }

        public void StateFixedUpdate()
        {

        }

        public void StateUpdate()
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
        }

        public void StateExit()
        {

        }

        public void StateFixedUpdate()
        {

        }

        public void StateUpdate()
        {
            _unit.gameObject.SetActive(true);
        }
    }


}
