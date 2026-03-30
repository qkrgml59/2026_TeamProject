using Item;
using System.Collections.Generic;
using Unit;
using UnityEngine;

[System.Serializable]
public class StageUnitData
{
    [Header("타일 위치")] public Vector2Int offset;
    [Header("유닛 프리팹")] public UnitBase unit;
    [Header("성급 (0 = 1성)")] public int star = 0;
    [Header("아이템 정보(아직 사용 안함)")] public List<ItemSO> items = new();
}
