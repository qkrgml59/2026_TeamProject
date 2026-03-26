using UnityEngine;
using Unit;

namespace Prototype.Grid
{
    public interface ITile
    {
        public UnitBase OccupantUnit { get; }
        public bool CanEnter(UnitBase unit);
        public void EnterTile(UnitBase unit);
        public void ExitTile(UnitBase unit);
    }
}
