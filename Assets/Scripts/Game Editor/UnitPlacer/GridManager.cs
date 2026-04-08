using Prototype.Grid;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Utilitys;


namespace GameEditor.UnitPlacer
{
    public class GridManager : SingletonMonoBehaviour<GridManager>
    {
        [Header("그리드 설정")]
        [SerializeField] private int column = 7;
        [SerializeField] private int row = 8;
        public HexTile[,] map { get; private set; }

        [Header("타일 설정")]
        public HexTile hexTilePrefab;

        [Header("타일 피벗 설정")]
        public Transform tilePivot;

        void Start()
        {
            GridInit();
        }

        /// <summary>
        /// 그리드 초기화
        /// </summary>
        [ContextMenu("그리드 재생성")]
        public void GridInit()
        {
            GridDestroy();
            GenerateGrid();
            Debug.Log("그리드 초기화 완료");
        }

        private void GenerateGrid()
        {
            if (column <= 0 || row <= 0)
            {
                Debug.LogWarning("올바르지 않은 그리드 크기 입니다.");
                return;
            }

            if (tilePivot == null)
            {
                Debug.LogError("Tile Pivot이 설정되어있지 않습니다.");
                return;
            }

            map = new HexTile[column, row];

            for (int col = 0; col < map.GetLength(0); col++)
                for (int row = 0; row < map.GetLength(1); row++)
                {
                    HexTile tile = Instantiate(hexTilePrefab);
                    tile.name = $"HexTile [{col}, {row}]";
                    tile.transform.SetParent(tilePivot);
                    Vector3 pos = HexMath.GetWorldPosition(col, row, 200) * 0.6f;
                    tile.transform.localPosition = new Vector2(pos.x, pos.z);
                    tile.transform.localScale = Vector3.one;

                    tile.offset = new Vector2Int(col, row);
                    map[col, row] = tile;
                }

            Debug.Log($"{column} x {row} 사이즈의 그리드 생성");
        }

        private void GridDestroy()
        {
            if (tilePivot == null)
            {
                Debug.LogError("Tile Pivot이 설정되어있지 않습니다.");
                return;
            }

            HexTile[] tiles = tilePivot.GetComponentsInChildren<HexTile>();


            foreach (var tile in tiles)
            {
                if (!Application.isPlaying)
                {
                    #if UNITY_EDITOR
                    DestroyImmediate(tile.gameObject);
                    #endif
                }
                else
                {
                    Destroy(tile.gameObject);
                }
            }

            map = null;
        }

        [ContextMenu("그리드 초기화")]
        public void GridReset()
        {
            if (map == null) return;

            for (int col = 0; col < map.GetLength(0); col++)
                for (int row = 0; row < map.GetLength(1); row++)
                {
                    if (map[col, row] == null) continue;

                    map[col, row].ResetTile();
                }
        }

        public void SetUnits(List<StageUnitData> units)
        {
            GridReset();

            foreach (var data in units)
            {
                HexTile tile = GetTile(data.offset - new Vector2Int(0, 4));         // 실제 적 위치와 row 값이 4 차이 나는걸 보정
                if (tile == null) continue;
                
                tile.SetUnitData(data);
            }

            Debug.Log("배치 정보 로드 완료");
        }

        public bool IsInBounds(Vector2Int offset)
        {
            return offset.x >= 0 &&
                   offset.x < map.GetLength(0) &&
                   offset.y >= 0 &&
                   offset.y < map.GetLength(1);
        }

        public HexTile GetTile(Vector2Int offset)
        {
            if (!IsInBounds(offset))
                return null;

            return map[offset.x, offset.y];
        }

        public HexTile GetTile(Vector3Int cube)
        {
            return GetTile(HexMath.CubeToOffset(cube));
        }
    }
}
