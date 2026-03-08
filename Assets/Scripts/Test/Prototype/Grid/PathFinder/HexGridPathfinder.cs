using System.Collections.Generic;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Utilitys;

namespace Prototype.Grid.Pathfind
{
    public class HexGridPathfinder : MonoBehaviour
    {
        [Header("길찾기 비용 설정")]
        [SerializeField] private int horizontalCost = 8;
        [SerializeField] private int verticalCost = 10;

        // 탐색 후보 타일
        private PriorityQueue<HexTile> open = new PriorityQueue<HexTile>();

        // 탐색 완료된 타일
        private HashSet<HexTile> closed = new HashSet<HexTile>();

        // 경로 복원용 부모 타일
        private Dictionary<Vector2Int, HexTile> parents = new Dictionary<Vector2Int, HexTile>();

        // 시작 -> 현재까지 실제 비용
        private Dictionary<Vector2Int, int> cost = new Dictionary<Vector2Int, int>();


        public bool TryGetPath(HexTile start, HexTile goal, List<HexTile> result)
        {
            // 데이터 초기화
            open.Clear();
            closed.Clear();
            parents.Clear();
            cost.Clear();
            result.Clear();
            return true;
        }

        int GetHeuristic(HexTile currentTile, HexTile goal)
        {
            int xDist = Mathf.Abs(currentTile.offset.x - goal.offset.x);
            int yDist = Mathf.Abs(currentTile.offset.y - goal.offset.y);

            return xDist * horizontalCost + yDist * verticalCost;
        }

        /// <summary>
        /// 해당 경로가 유효한지 확인
        /// </summary>
        public bool IsPathStillValid(List<HexTile> path)
        {
            foreach (var tile in path)
            {
                if (!tile.CanEnter())
                    return false;
            }

            return true;
        }
    }
}
