using UnityEngine;
using Prototype.Grid;
using Unit;
using Prototype.UI;

public class TestEnemyPlacer : MonoBehaviour
{
    private const TeamType teamType = TeamType.Enemy;

    void Start()
    {
        BattleManager.Instance.OnRoundStart += Place;
    }

    [ContextMenu("유닛 배치")]
    public void Place()
    {
        RoundData round = StageManager.Instance.CurrentRound;

        if(round == null) return;


        // 라운드 데이터를 따라 적 배치
        foreach(var u in round.units)
        {
            HexTile targetTile = GridManager.Instance.GetTile(u.offset);

            if (targetTile != null && targetTile.CanReserve(null))
            {
                UnitBase target = Instantiate(u.unit, targetTile.transform.position, Quaternion.identity);
                target.team = teamType;
                target.PlaceUnit(targetTile);
                UnitManager.Instance.RegisterUnit(target);
                IndicatorManager.Instance.HPBarPresenter.RegisterHealthBar(target);

                // TODO : 아이템이 있다면 장착
            }
        }

    }
}
