using UnityEngine;
using Prototype.Grid;
using Unit;
using Prototype.UI;
using Item;

public class TestEnemyPlacer : MonoBehaviour
{
    public UnitBase unitPrefab;
    private const TeamType teamType = TeamType.Enemy;

    void Start()
    {
        if (BattleManager.Instance != null)
            BattleManager.Instance.OnRoundStart += Place;
    }

    private void OnDestroy()
    {
        if (BattleManager.Instance != null)
            BattleManager.Instance.OnRoundStart -= Place;
    }

    [ContextMenu("유닛 배치")]
    public void Place(RoundData round)
    {
        if(round == null || unitPrefab == null) return;


        // 라운드 데이터를 따라 적 배치
        foreach(var u in round.units)
        {
            HexTile targetTile = GridManager.Instance.GetTile(u.offset);

            if (targetTile != null && targetTile.CanReserve(null))
            {
                UnitBase target = UnitSpawner.Instance.SpawnUnit(u.unitData, targetTile, TeamType.Enemy, u.star);

                if (target == null) continue;

                // 아이템이 있다면 장착
                foreach(var item in u.items)
                {
                    ItemBase newItem = new ItemBase(item);
                    target.TryEquippedItem(newItem);
                }
            }
        }

    }
}
