using UnityEngine;

public class HexTile : MonoBehaviour
{
    // Axial 좌표 (알고리즘 계산용)
    public Vector2Int axialCoord;

    // Offset 좌표 (그리드상의 행/열 위치, 디버깅용)
    public Vector2Int offsetCoord;

    public void Init(int q, int r, int col, int row)
    {
        this.axialCoord = new Vector2Int(q, r);
        this.offsetCoord = new Vector2Int(col, row);

        // 이름 변경
        this.name = $"Hex [{q}, {r}] / Off({col},{row})";
    }
}
