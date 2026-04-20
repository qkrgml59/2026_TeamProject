using System.Collections.Generic;
using Unit;
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
        private readonly PriorityQueue<HexTile> open = new PriorityQueue<HexTile>();

        // 탐색 완료된 타일
        private readonly HashSet<HexTile> closed = new HashSet<HexTile>();

        // 경로 복원용 부모 타일
        private readonly Dictionary<HexTile, HexTile> parents = new Dictionary<HexTile, HexTile>();

        // 시작 -> 현재까지 실제 비용
        private readonly Dictionary<HexTile, int> gCost = new Dictionary<HexTile, int>();


        public bool TryGetPath(HexTile start, HexTile goal, List<HexTile> result, float attackRange)
        {
            if(start == null || goal == null || result == null) return false;

            // 데이터 초기화
            open.Clear();
            closed.Clear();
            parents.Clear();
            gCost.Clear();
            result.Clear();

            // 시작 타일 지정
            gCost[start] = 0;
            open.Enqueue(start, GetHeuristic(start, goal));

            while(open.Count > 0)
            {
                HexTile current = open.Dequeue();


                // 이미 최단 경로로 도달한 타일인 경우
                if (closed.Contains(current))
                    continue;

                // 현재 타일이 목표 타일인 경우
                if (current == goal)
                {
                    // 완료 처리
                    return TryGetBuildPath(start, goal, result);
                }

                if(HexMath.Distance(current.Offset, goal.Offset) <= attackRange)
                {
                    // 공격 가능 범위에 들어오면 공격
                    return TryGetBuildPath(start, current, result);
                }

                closed.Add(current);

                // Offset -> Cube 좌표 변환
                Vector3Int cubeCoord = HexMath.OffsetToCube(current.Offset);

                foreach (Vector3Int neighborDir in HexMath.CubeDirections)
                {
                    HexTile neighbor = GridManager.Instance.GetTile(cubeCoord + neighborDir);

                    if (neighbor == null) continue;
                    if (!neighbor.CanReserve(null)&& neighbor != goal) continue;
                    if (closed.Contains(neighbor)) continue;

                    int cost = gCost[current] + GetMoveCost(current, neighbor);

                    // 기존 비용보다 저렴하게 도달한 경우
                    if(!gCost.TryGetValue(neighbor, out int oldG) || cost < oldG)
                    {
                        gCost[neighbor] = cost;              // 비용 갱신
                        parents[neighbor] = current;        // 경로(부모) 갱신

                        int f = cost + GetHeuristic(neighbor, goal);
                        open.Enqueue(neighbor, f);
                    }
                }
            }

            return false;
        }

        private bool TryGetBuildPath(HexTile start, HexTile goal, List<HexTile> result)
        {
            result.Clear();

            HexTile current = goal;
            result.Add(current);

            while (true)
            {
                // 부모를 찾을 수 없는 경우 오류 반환
                if (!parents.ContainsKey(current))
                {
                    Debug.LogError("경로 타일을 찾을 수 없음");
                    result.Clear();
                    return false;
                }

                // 부모를 현재 타일로 설정
                current = parents[current];

                if (current == start)
                    break;

                result.Add(current);
            }

            result.Reverse();
            return true;
        }

        int GetMoveCost(HexTile current, HexTile next)
        {
            if (current.Offset.y == next.Offset.y) return horizontalCost;
            else return verticalCost;
        }

        int GetHeuristic(HexTile currentTile, HexTile goal)
        {
            return HexMath.Distance(currentTile.Offset, goal.Offset);
        }

        /// <summary>
        /// 해당 경로가 유효한지 확인
        /// </summary>
        public bool IsPathStillValid(List<HexTile> path, int currentIndex, UnitBase targetUnit, float attackRange)
        {
            if (path == null || path.Count == 0)
                return false;

            for (int i = currentIndex; i < path.Count; i++)
            {
                // 진입 불가능 + 점유 유닛이 타겟도 아니면 불가능
                if (!path[i].CanReserve(null) && path[i].ReservedUnit != targetUnit && path[i].OccupantUnit != targetUnit)
                    return false;

                // 이동 중 대상이 사거리 안에 있어도 유효한 경로
                if (HexMath.Distance(path[i].Offset, targetUnit.offset) <= attackRange)
                    return true;
            }

            // 도착지에서 사거리 안에 적이 없는 경우
            if (HexMath.Distance(path[^1].Offset, targetUnit.offset) > attackRange)
                return false;

            return true;
        }
    }
}
