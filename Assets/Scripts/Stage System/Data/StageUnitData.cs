using Item;
using System.Collections.Generic;
using Unit;
using UnityEngine;

[System.Serializable]
public class StageUnitData
{
    [Header("타일 위치")] public Vector2Int offset;
    [Header("유닛 프리팹")] public UnitDataSO unitData;
    [Header("성급 (0 = 1성)")] public int star = 0;
    [Header("아이템 정보(아직 사용 안함)")] public List<ItemSO> items = new();

    public void Clear()
    {
        offset = Vector2Int.zero;
        unitData = null;
        star = -1;
        items.Clear();
    }

    public StageUnitData Clone()
    {
        StageUnitData copy = new StageUnitData();
        copy.offset = offset;
        copy.unitData = unitData;
        copy.star = star;
        copy.items = items;
        return copy;
    }
}
