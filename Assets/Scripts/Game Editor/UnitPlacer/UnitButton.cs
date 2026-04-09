using TMPro;
using Unit;
using UnityEngine;
using UnityEngine.UI;

namespace GameEditor.UnitPlacer
{
    public class UnitButton : DragButtonBase
    {
        [Header("유닛 버튼 컴포턴트")]
        public Image icon;
        public TextMeshProUGUI buttonName;

        [Header("유닛 데이터")]
        public UnitDataSO data;

        [Header("값이 빈 경우 디폴드 데이터들")]
        public Sprite defaultIcon;
        public string defaultName = "빈 데이터";

        public void Init(Canvas canvas, UnitDataSO unitData)
        {
            Init(canvas);
            data = unitData;
            icon.sprite = unitData.unitSprite ?? defaultIcon;
            buttonName.text = unitData.Name_KR ?? defaultName;
        }

        protected override void Use(HexTile tile)
        {
            if (data == null)
                return;

            tile.SetNewUnit(data);
        }
    }
}
