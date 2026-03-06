using UnityEngine;
using Tools;

namespace Prototype.Grid
{
    /// <summary>
    /// 그리드를 관리하는 매니저입니다.
    /// </summary>
    public class GridManager : SingletonMonoBehaviour<GridManager>
    {
        [Header("그리드 정보")]
        [SerializeField] private int column = 7;
        [SerializeField] private int row = 8;
        private HexTile[,] map;

        void Start()
        {
            GenerateGrid();
        }

        public void GenerateGrid()
        {
            Debug.Log($"{column} x {row} 사이즈의 그리드 생성");
            map = new HexTile[7, 8];
        }
    }
}
