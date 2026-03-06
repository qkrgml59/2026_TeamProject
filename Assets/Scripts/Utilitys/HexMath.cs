using UnityEngine;

namespace Prototype.Grid
{
    public enum HexDirection
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
            new Vector3Int(1, 0, -1),   // TopRight
            new Vector3Int(0, 1, -1),   // TopLeft
            new Vector3Int(-1, 1, 0),   // Left
            new Vector3Int(-1, 0, 1),   // BottomLeft
            new Vector3Int(0, -1, 1)    // BottomRight
        };

        /// <summary>
        /// Cube 좌표계에서의 특정 방향을 반환
        /// </summary>
        public static Vector3Int GetCubeDirection(HexDirection dir) => CubeDirections[(int)dir];
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
        public static int CubeDistance(Vector2Int a, Vector2Int b)
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
    }
}
