using UnityEngine;

namespace Prototype.Grid
{
    /// <summary>
    /// 육각 그리드의 런타임 데이터
    /// </summary>
    [System.Serializable]
    public class HexGrid
    {
        [Header("그리드 정보")]
        public int column;
        public int row;
        public HexTile[,] map { get; private set; }
    }
}
