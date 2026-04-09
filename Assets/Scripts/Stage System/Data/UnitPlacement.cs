using Item;
using System.Collections.Generic;
using Unit;
using UnityEngine;

/// <summary>
/// 유닛의 배치 정보
/// </summary>
[System.Serializable]
public class UnitPlacement
{
    [Header("타일 위치")] public Vector2Int offset;
    [Header("유닛 프리팹")] public UnitDataSO unitData;
    [Header("성급 (0 = 1성)")] public int star = 0;
    [Header("아이템 정보(아직 사용 안함)")] public List<ItemSO> items = new();

    /// <summary>
    /// 배치 툴에서 데이터를 초기값으로 설정
    /// </summary>
    /// <param name="offset"></param>
    public void Clear(Vector2Int offset)
    {
        this.offset = offset + new Vector2Int(0, 4);            // 실제 위치와 차이 보정
        unitData = null;
        star = -1;
        items.Clear();
    }


    // 변경이 필요할 경우 생성자로 복제하여 사용
    public UnitPlacement(UnitPlacement other)
    {
        offset = other.offset;
        unitData = other.unitData;
        star = other.star;
        items = new List<ItemSO>(other.items);
    }
}
