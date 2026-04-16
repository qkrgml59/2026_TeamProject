using UnityEngine;
using Prototype.Grid;
using Unit;
using Prototype.UI;

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
                UnitBase target = Instantiate(unitPrefab, targetTile.transform.position, Quaternion.identity);
                target.Init(u.unitData, teamType);
                target.PlaceUnit(targetTile);
                UnitManager.Instance.RegisterUnit(target);
                UnitUIManager.Instance.Create(target);
                target.RecalculateUnitStats();

                // TODO : 아이템이 있다면 장착
            }
        }

    }
}
