using UnityEngine;
using Unit;

namespace Prototype.Grid
{
    /// <summary>
    /// 육각 그리드 타일
    /// </summary>
    public class HexTile : MonoBehaviour
    {
        public Vector2Int Offset { get; private set; }
        public UnitBase OccupantUnit { get; private set; }
        public UnitBase ReservedUnit { get; private set; }

        public TeamType TeamType { get; private set; }

        // 타일 프리뷰 랜더러
        Renderer hexRenderer;

        private void Awake()
        {
            hexRenderer = GetComponentInChildren<Renderer>();
            if(hexRenderer != null ) hexRenderer.enabled = false;
        }

        public void Init(Vector2Int offset, TeamType team)
        {
            this.Offset = offset;
            TeamType = team;
        }

        /// <summary>
        /// 진입 가능한지 여부
        /// </summary>
        public bool CanReserve(UnitBase unit)
        {
            return OccupantUnit == null && ReservedUnit == null;
        }

        /// <summary>
        /// 진입 시도
        /// </summary>
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
            if (OccupantUnit == unit || OccupantUnit == null)
                OccupantUnit = null;
            else
                Debug.LogError($"[{Offset} 타일] 유닛 데이터 에러 (대상 유닛: {unit.name} | 실점유 유닛: {OccupantUnit?.name})");
        }

        public void ResetTile()
        {
            OccupantUnit = null;
            ReservedUnit = null;
        }

        private void OnEnable()
        {
            if(GridManager.Instance != null)
            {
                GridManager.Instance.OnBeginDrag += OnBeginDrag;
                GridManager.Instance.OnEndDrag += OnEndDrag;
            }
        }

        private void OnDisable()
        {
            if (GridManager.Instance != null)
            {
                GridManager.Instance.OnBeginDrag -= OnBeginDrag;
                GridManager.Instance.OnEndDrag -= OnEndDrag;
            }
        }

        void OnBeginDrag(TeamType team)
        {
            if (team == TeamType.Both || team == TeamType)          // 둘다 가능하거나 타일에 해당하는 영역이라면
                ActiveRender(true);                                 // 프리뷰 활성화
            else
                ActiveRender(false);
        }

        void OnEndDrag()
        {
            ActiveRender(false);
        }

        void ActiveRender(bool active)
        {
            if(hexRenderer != null) hexRenderer.enabled = active;
        }
    }

}
