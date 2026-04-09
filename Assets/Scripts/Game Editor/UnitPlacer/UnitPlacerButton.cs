using TMPro;
using Unit;
using UnityEngine;
using UnityEngine.UI;

namespace GameEditor.UnitPlacer
{
    public class UnitPlacerButton : PlacerButton
    {
        public UnitDataSO data;
        public Image icon;
        public TextMeshProUGUI buttonName;
        public void Init(Canvas canvas, UnitDataSO unitData)
        {
            base.Init(canvas);
            data = unitData;
            icon.sprite = unitData.unitSprite;
            buttonName.text = unitData.Name_KR;
        }
        protected override void Use(HexTile tile)
        {
            tile.SetNewUnit(data);
        }
    }
}
