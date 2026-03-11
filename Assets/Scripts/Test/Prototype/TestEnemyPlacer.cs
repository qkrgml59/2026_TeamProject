using UnityEngine;
using Prototype.Grid;
using Prototype.Unit;
using System.Collections.Generic;

public class TestEnemyPlacer : MonoBehaviour
{
    public UnitBase meleeUnit;
    public UnitBase rangedUnit;
    public List<Vector2Int> offsetList = new(); 

    private const TeamType teamType = TeamType.Enemy;


    [ContextMenu("유닛 배치")]
    public void Place()
    {
        if(rangedUnit == null || meleeUnit == null) return;

        foreach(Vector2Int offset in offsetList)
        {
            if (!GridManager.Instance.IsInBounds(offset)) return;

            HexTile targetTile = GridManager.Instance.GetTile(offset);

            if (targetTile != null && targetTile.CanEnter())
            {
                int rand = Random.Range(0, 2);
                UnitBase target = Instantiate(rand == 0 ? meleeUnit : rangedUnit, targetTile.transform.position, Quaternion.identity);
                target.team = teamType;
                targetTile.EnterTile(target);
                target.EnterTile(targetTile);
                UnitManager.Instance.RegisterUnit(target);
            }
        }

    }
}
