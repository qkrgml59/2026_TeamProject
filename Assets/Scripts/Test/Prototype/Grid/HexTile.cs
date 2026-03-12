using UnityEngine;
using Prototype.Unit;

namespace Prototype.Grid
{
    /// <summary>
    /// 육각 그리드 타일
    /// </summary>
    public class HexTile : MonoBehaviour
    {
        public Vector2Int offset;
        public UnitBase OccupantUnit { get; private set; }
        public UnitBase ReservedUnit { get; private set; }

        public bool CanReserve(UnitBase unit)
        {
            return OccupantUnit == null && ReservedUnit == null;
        }

        public bool TryReserve(UnitBase unit)
        {
            if (!CanReserve(unit))
                return false;

            ReservedUnit = unit;
            return true;
        }

        public void CancelReserve(UnitBase unit)
        {
            if (ReservedUnit == unit)
                ReservedUnit = null;
        }

        public void EnterTile(UnitBase unit)
        {
            OccupantUnit = unit;

            if (ReservedUnit == unit)
                ReservedUnit = null;
        }

        public void ExitTile(UnitBase unit)
        {
            if (OccupantUnit == unit)
                OccupantUnit = null;
            else
                Debug.LogError($"[{offset} 타일] 유닛 데이터 에러 (대상 유닛: {unit.name} | 실점유 유닛: {OccupantUnit.name})");
        }

        public void ResetTile()
        {
            OccupantUnit = null;
            ReservedUnit = null;
        }
    }

}
