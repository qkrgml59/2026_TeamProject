using UnityEngine;

namespace Prototype.Grid
{
    public enum HexDirectionType
    {
        Right,
        TopRight,
        TopLeft,
        Left,
        BottomLeft,
        BottomRight
    }

    /// <summary>
    /// 육각 그리드와 관련된 계산 기능을 모아둔 클래스 입니다.
    /// </summary>
    public static class HexMath
    {
        #region Directions Preset
        /// <summary>
        /// Cube 좌표계에서의 6방향
        /// </summary>
        public static readonly Vector3Int[] CubeDirections =
        {
            new Vector3Int(1, -1, 0),   // Right
            new Vector3Int(-1, 1, 0),   // Left
            new Vector3Int(1, 0, -1),   // TopRight
            new Vector3Int(0, 1, -1),   // TopLeft
            new Vector3Int(-1, 0, 1),   // BottomLeft
            new Vector3Int(0, -1, 1)    // BottomRight
        };

        /// <summary>
        /// Cube 좌표계에서의 특정 방향을 반환
        /// </summary>
        public static Vector3Int GetCubeDirection(HexDirectionType dir) => CubeDirections[(int)dir];

        #endregion

        #region Coordinate Convert
        /// <summary>
        /// Offset 좌표를 Cube 좌표로 변환 후 반환합니다.
        /// </summary>
        public static Vector3Int OffsetToCube(Vector2Int offset)
        {
            int x = offset.x - (offset.y - (offset.y & 1)) / 2;         // offset.y & 1 비트 연산, 마지막 자리가 1인지 확인해 홀짝 구분
            int z = offset.y;
            int y = -x - z;

            return new Vector3Int(x, y, z);
        }

        /// <summary>
        /// Cube 좌표를 Offset 좌표로 변환 후 반환합니다.
        /// </summary>
        public static Vector2Int CubeToOffset(Vector3Int cube)
        {
            int col = cube.x + (cube.z - (cube.z & 1)) / 2;
            int row = cube.z;

            return new Vector2Int(col, row);
        }
        #endregion

        #region Distance
        /// <summary>
        /// 두 위치간의 거리를 반환합니다.
        /// </summary>
        public static int Distance(Vector3Int a, Vector3Int b)
        {
            return (Mathf.Abs(a.x - b.x)
                    + Mathf.Abs(a.y - b.y)
                    + Mathf.Abs(a.z - b.z)) / 2;
        }

        /// <summary>
        /// 두 위치간의 거리를 반환합니다.
        /// </summary>
        public static int Distance(Vector2Int a, Vector2Int b)
        {
            return Distance(OffsetToCube(a), OffsetToCube(b));
        }
        #endregion

        public static Vector3 GetWorldPosition(int col, int row, float tileSize)
        {
            float width = Mathf.Sqrt(3f) * tileSize;

            float x = width * (col + 0.5f * (row & 1));
            float z = tileSize * 1.5f * row;

            return new Vector3(x, 0f, z);
        }

        /// <summary>
        /// 공격자 offset -> 타겟 offset 방향을
        /// 가장 가까운 헥스 6방향 중 하나로 반환
        /// </summary>
        public static Vector3Int GetDirectionCube(Vector2Int fromOffset, Vector2Int toOffset)
        {
            Vector3Int fromCube = OffsetToCube(fromOffset);
            Vector3Int toCube = OffsetToCube(toOffset);

            Vector3Int delta = toCube - fromCube;

            // 인접 타일이면 거의 그대로 방향 벡터가 나옴
            foreach (var dir in CubeDirections)
            {
                if (delta == dir)
                    return dir;
            }

            // 멀리 있는 경우 가장 가까운 방향 선택
            Vector3Int bestDir = CubeDirections[0];
            int bestDot = int.MinValue;

            foreach (var dir in CubeDirections)
            {
                int dot = delta.x * dir.x + delta.y * dir.y + delta.z * dir.z;
                if (dot > bestDot)
                {
                    bestDot = dot;
                    bestDir = dir;
                }
            }

            return bestDir;
        }

        /// <summary>
        /// 공격자 cube -> 타겟 cube 방향을
        /// 가장 가까운 헥스 6방향 중 하나로 반환
        /// </summary>
        public static Vector3Int GetDirectionCube(Vector3Int fromCube, Vector3Int toCube)
        {
            return GetDirectionCube(CubeToOffset(fromCube), CubeToOffset(toCube));
        }
    }
}
