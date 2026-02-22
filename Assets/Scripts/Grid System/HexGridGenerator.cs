using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    public class HexGridGenerator : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("타일 프리팹")] public GameObject tilePrefab; // 육각형 타일 프리팹
        [Tooltip("가로 크기")] public int width = 7;         // 가로 칸 수
        [Tooltip("세로 크기")] public int height = 8;        // 세로 칸 수
        [Tooltip("타일 반지름")] public float hexSize = 1.0f;  // 육각형 반지름

        [Header("간격")]
        [Tooltip("타일 간 가로 여백")] public float xPadding = 0.1f; // 타일 간 가로 여백
        [Tooltip("타일 간 세로 여백")] public float zPadding = 0.1f; // 타일 간 세로 여백

        // 좌표로 타일을 찾기 위한 딕셔너리
        public Dictionary<Vector2Int, HexTile> TileMap = new Dictionary<Vector2Int, HexTile>();

        void Start()
        {
            GenerateBoard();
        }

        // 그리드 생성
        public void GenerateBoard()
        {
            // 타일 초기화
            foreach (Transform child in transform) Destroy(child.gameObject);
            TileMap.Clear();

            // 육각형 사이즈 계산
            // 육각형의 (Width) = size * 2
            // 육각형의 (Height) = size * sqrt(3)        // sqrt = 제곱근
            float hexWidth = hexSize * 2f;
            float hexHeight = hexSize * Mathf.Sqrt(3f);

            // 수평/수직 거리 (간격)
            // 가로는 3/4(75%) 지점에서 다음 타일이 시작
            float horizDist = (hexWidth * 0.75f) + xPadding;
            float vertDist = hexHeight + zPadding;

            // 직사각형 채우기
            for (int row = 0; row < height; row++)              // row = 행(가로)
            {
                for (int col = 0; col < width; col++)           // col = 열(세로)
                {
                    // 위치 계산 (World Position)

                    float xPos = col * horizDist;

                    float x = col * (hexWidth * 0.75f); // 가로 간격
                    float z = row * (hexHeight);        // 세로 간격

                    float xWorld = col * (hexSize * 1.5f);
                    float zWorld = row * (hexSize * Mathf.Sqrt(3f));

                    // 홀수 컬럼(col)일 때, z위치를 반 칸 올림
                    if (col % 2 == 1)
                    {
                        zWorld += (hexSize * Mathf.Sqrt(3f)) / 2f;
                    }

                    // 4. Axial 좌표 변환
                    int q = col;
                    int r = row - (col - (col & 1)) / 2;


                    // 5. 타일 생성
                    Vector3 finalPos = new Vector3(xWorld, 0, zWorld);
                    SpawnTile(q, r, col, row, finalPos);
                }
            }

            // 카메라를 중앙 이동
            CenterCamera();
        }

        // 타일 생성 
        void SpawnTile(int q, int r, int col, int row, Vector3 pos)
        {
            GameObject obj = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
            HexTile tileScript = obj.AddComponent<HexTile>();

            tileScript.Init(q, r, col, row);

            // Dictionary 등록
            Vector2Int key = new Vector2Int(q, r);
            if (!TileMap.ContainsKey(key))
            {
                TileMap.Add(key, tileScript);
            }
        }

        void CenterCamera()
        {
            if (Camera.main == null) return;

            float centerX = (width * hexSize * 1.5f) / 2f;
            float centerZ = (height * hexSize * Mathf.Sqrt(3f)) / 2f;

            Camera.main.transform.position = new Vector3(centerX, 10, centerZ - 5);
            Camera.main.transform.LookAt(new Vector3(centerX, 0, centerZ));
        }
    }

}
