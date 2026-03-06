using UnityEngine;
using System;

namespace Prototype.Grid
{
    /// <summary>
    /// Offset 좌표 (저장용)
    /// </summary>
    public class OffsetCoord
    {
        public int col;   // x
        public int row;   // z

        public OffsetCoord(int col, int row)
        {
            this.col = col;
            this.row = row;
        }
    }

    /// <summary>
    /// Cube 좌표 (계산용)
    /// </summary>
    public class CubeCoord
    {
        public Vector3Int value;

        public int X => value.x;
        public int Y => value.y;
        public int Z => value.z;

        public CubeCoord(int x, int y, int z)
        {
            // 큐브 좌표는 항상 x + y + z = 0, 아닐시 throw
            if (x + y + z != 0)
                throw new Exception();

            value = new Vector3Int(x, y, z);
        }
    }
}
