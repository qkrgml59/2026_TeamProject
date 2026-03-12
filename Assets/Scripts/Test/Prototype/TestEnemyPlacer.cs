using UnityEngine;
using Prototype.Grid;
using Prototype.Unit;
using Prototype.UI;

public class TestEnemyPlacer : MonoBehaviour
{
    public UnitBase meleeUnit;
    public UnitBase rangedUnit;
    public int enemyCount = 2;

    private const TeamType teamType = TeamType.Enemy;

    void Start()
    {
        BattleManager.Instance.OnRoundStart += Place;
    }

    [ContextMenu("유닛 배치")]
    public void Place()
    {
        if(rangedUnit == null || meleeUnit == null) return;

        int colSize = GridManager.Instance.map.GetLength(0);
        int rowMax = GridManager.Instance.map.GetLength(1);
        int rowMin = rowMax / 2;


        for (int i = 0; i < enemyCount;)
        {
            int randCol = Random.Range(0, colSize );
            int randRow = Random.Range(rowMin, rowMax);
            Vector2Int offset = new Vector2Int(randCol, randRow);
            
            HexTile targetTile = GridManager.Instance.GetTile(offset);

            if (targetTile != null && targetTile.CanReserve(null))
            {
                UnitBase target = Instantiate(randRow < rowMin + 2? meleeUnit : rangedUnit, targetTile.transform.position, Quaternion.identity);
                target.team = teamType;
                targetTile.EnterTile(target);
                target.EnterTile(targetTile);
                UnitManager.Instance.RegisterUnit(target);
                IndicatorManager.Instance.HPBarPresenter.RegisterHealthBar(target);
                i++;
            }
        }

    }
}
