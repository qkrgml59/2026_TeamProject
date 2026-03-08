using UnityEngine;

namespace Prototype.Grid
{
    public interface ITile
    {
        public UnitBase OccupantUnit { get; }
        public bool CanEnter();
        public void EnterTile(UnitBase unit);
        public void ExitTile(UnitBase unit);
    }
}
