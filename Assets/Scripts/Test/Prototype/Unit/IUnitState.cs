using UnityEngine;

namespace Prototype.Unit
{
    public interface IUnitState
    {
        public void StateEnter();
        public void StateUpdate();
        public void StateFixedUpdate();
        public void StateExit();
    }
}
