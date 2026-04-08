using Item;
using System.Collections.Generic;
using TMPro;
using Unit;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GameEditor.UnitPlacer
{
    public class HexTile : MonoBehaviour
    {
        [Header("컴포넌트 설정")]
        public TextMeshProUGUI unitName;
        public Image unitImage;
        public List<GameObject> stars = new();
        public List<GameObject> itemIcons = new();

        [Header("타일 정보")]
        public Vector2Int offset;

        [Header("데이터")]
        public UnitDataSO occupantUnit;
        public int unitStar = 1;
        public List<ItemSO> items = new();

        public void SetUnit(UnitDataSO unit)
        {
            if (occupantUnit != null || unit == null) return;
            occupantUnit = unit;

            unitName.text = unit.Name_KR;
            unitName.enabled = true;

            unitImage.sprite = unit.unitSprite;
            unitImage.enabled = true;
        }

        public void RemoveUnit()
        {
            if (occupantUnit == null) return;
            occupantUnit = null;

        }


        public void ChageTileToTile(HexTile tile)
        {

        }

        public void ResetTile()
        {
            occupantUnit = null;
            unitStar = 1;
            items.Clear();

            // 이미지 삭제 등
        }
    }
}
