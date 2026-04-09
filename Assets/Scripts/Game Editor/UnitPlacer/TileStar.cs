using GameEditor.UnitPlacer;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileStar : MonoBehaviour, IPointerClickHandler
{
    public HexTile targetTile;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (targetTile == null) return;

        if (eventData.button == PointerEventData.InputButton.Left)
            targetTile.UpgradeStar();
        else if (eventData.button == PointerEventData.InputButton.Right)
            targetTile.PreviousStar();
    }
}
