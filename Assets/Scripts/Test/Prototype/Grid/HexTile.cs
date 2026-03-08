using UnityEngine;

namespace Prototype.Grid
{
    /// <summary>
    /// 육각 그리드 타일
    /// </summary>
    public class HexTile : MonoBehaviour, ITile
    {
        public Vector2Int offset;

        public UnitBase OccupantUnit { get; private set; }

        public bool CanEnter()
        {
            // TODO: 접근 불가능한 조건이 있다면 여기에 추가
            return OccupantUnit == null;
        }

        public void EnterTile(UnitBase unit)
        {
            OccupantUnit = unit;
        }

        public void ExitTile(UnitBase unit)
        {
            if (OccupantUnit != null && OccupantUnit != unit)
                Debug.LogError($"[{offset} 타일] 유닛 데이터 에러 (대상 유닛: {unit.name} | 실점유 유닛: {OccupantUnit.name})");

            OccupantUnit = null;
        }
    }

}
