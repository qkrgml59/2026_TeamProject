using Unit;
using UnityEngine;

namespace GameEditor.UnitPlacer
{
    public class UnitPlacerButton : PlacerButton
    {
        public UnitDataSO data;
        public void Init(Canvas canvas, UnitDataSO unitData)
        {
            base.Init(canvas);
            data = unitData;
            icon.sprite = unitData.unitSprite;
            buttonName.text = unitData.Name_KR;
        }
        protected override void Use(HexTile tile)
        {
            tile.SetUnit(data);
        }
    }
}
