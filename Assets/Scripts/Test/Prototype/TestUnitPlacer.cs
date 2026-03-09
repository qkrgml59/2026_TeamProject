using UnityEngine;
using Prototype.Grid;
using Prototype.Unit;

public class TestUnitPlacer : MonoBehaviour
{
    public UnitBase targetUnit;
    public TeamType teamType = TeamType.Ally;
    public HexTile targetTile;

    [ContextMenu("유닛 배치")]
    public void Place()
    {
        if(targetUnit == null || targetTile == null) return;

        if(targetTile.CanEnter())
        {
            UnitBase target = Instantiate(targetUnit, targetTile.transform.position, Quaternion.identity);
            target.team = teamType;
            targetTile.EnterTile(target);
            target.EnterTile(targetTile);
            UnitManager.Instance.RegisterUnit(target);
        }
    }
}
