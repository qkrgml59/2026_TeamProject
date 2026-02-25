using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    public static class HexUtils               //편의성 기능 작성할 클래스
    {
        public struct CubeCoord         //Axial 좌표계를 cube좌표계로 변환
        {
            public int x, y, z;
            public CubeCoord(int q, int r)
            {
                x = q;
                z = r;
                y = -x - z; //3번째 축 자동 계산
            }
        }

        /// <summary>
        /// 거리 계산
        /// 두 Axial 좌표 사이의 거리 반환
        /// </summary>
        public static int GetDistance(Vector2Int a, Vector2Int b)
        {
            {
                CubeCoord ca = new CubeCoord(a.x, a.y);
                CubeCoord cb = new CubeCoord(b.x, b.y);

                // 3축의 차이 절대값 합의 절반이 거리
                return (Mathf.Abs(ca.x - cb.x) + Mathf.Abs(ca.y - cb.y) + Mathf.Abs(ca.z - cb.z)) / 2;
            }
        }

        /// <summary>
        /// 인접 타일
        /// 특정 좌표의 주변 6칸 좌표를 반환
        /// </summary>
        public static List<Vector2Int> GetNeighbors(Vector2Int center)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            // Flat Top 기준 6방향 오프셋
            Vector2Int[] directions = new Vector2Int[]
            {
                new Vector2Int(1, -1),      //오른쪽 상단
                new Vector2Int(1, 0),       //오른쪽 중단
                new Vector2Int(0, 1),       //오른쪽 하단
                new Vector2Int(0, -1),      //왼쪽 상단
                new Vector2Int(-1, 0),      //왼쪽 중단
                new Vector2Int(-1, 1)       //왼쪽 하단
            };

            foreach (var dir in directions)
            {
                neighbors.Add(center + dir);
            }
            return neighbors;
        }
    }
}
