using UnityEngine;
using Tools;

namespace Prototype.Grid
{
    /// <summary>
    /// 그리드를 관리하는 매니저입니다.
    /// </summary>
    public class GridManager : SingletonMonoBehaviour<GridManager>
    {
        [Header("그리드 설정")]
        [SerializeField] private int column = 7;
        [SerializeField] private int row = 8;
        private HexTile[,] map;

        [Header("타일 설정")]
        [SerializeField] private float tileSize = 2f;

        [Header("디버그 설정")]
        public bool DebugMode = false;

        void Start()
        {
            GridInit();
        }

        /// <summary>
        /// 그리드 초기화
        /// </summary>
        [ContextMenu("그리드 초기화")]
        public void GridInit()
        {
            GridClear();
            GenerateGrid();
            Debug.Log("그리드 초기화 완료");
        }

        private void GenerateGrid()
        {
            if(column <= 0 || row <= 0)
            {
                Debug.LogWarning("올바르지 않은 그리드 크기 입니다.");
                return;
            }

            if (tileSize <= 0)
            {
                Debug.LogWarning("타일 크기는 양수여야 합니다.");
                return;
            }

            map = new HexTile[column, row];

            for (int col = 0; col < map.GetLength(0); col++)
                for (int row = 0; row < map.GetLength(1); row++)
                {
                    GameObject obj = new GameObject($"HexTile [{col}, {row}]");
                    obj.transform.SetParent(transform);

                    HexTile tile = obj.AddComponent<HexTile>();
                    tile.transform.position = transform.position + HexMath.GetWorldPosition(col, row, tileSize);
                    map[col, row] = tile;
                }

            Debug.Log($"{column} x {row} 사이즈의 그리드 생성");
        }

        private void GridClear()
        {
            if (map == null) return;

            for(int col = 0; col < map.GetLength(0); col++)
                for (int row = 0; row < map.GetLength(1); row++)
                {
                    if (map[col, row] == null) continue;

                    Destroy(map[col, row].gameObject);
                    map[col, row] = null;
                }
        }

        private void OnDrawGizmos()
        {
            if (tileSize <= 0) return;

            if (Application.isPlaying)
            {
                if (DebugMode && map != null)
                {
                    for (int col = 0; col < map.GetLength(0); col++)
                        for (int row = 0; row < map.GetLength(1); row++)
                            DrawHex(map[col, row].transform.position);
                }
            }
            else
            {
                for (int col = 0; col < column; col++)
                    for (int row = 0; row < this.row; row++)
                        DrawHex(transform.position + HexMath.GetWorldPosition(col, row, tileSize));
            }
        }

        void DrawHex(Vector3 center)
        {
            Vector3 prev = Vector3.zero;

            for (int i = 0; i <= 6; i++)
            {
                float angle = Mathf.Deg2Rad * (60 * i + 30);

                Vector3 pos = center + new Vector3(
                    Mathf.Cos(angle) * tileSize,
                    0,
                    Mathf.Sin(angle) * tileSize
                );

                if (i > 0)
                    Gizmos.DrawLine(prev, pos);

                prev = pos;
            }
        }
    }
}
