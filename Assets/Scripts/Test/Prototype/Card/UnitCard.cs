using Prototype.Card.Unit;
using Prototype.Grid;
using Prototype.UI;
using Unit;
using UnityEngine;

namespace Prototype.Card
{
    public class UnitCard : CardBase
    {
        public UnitBase unitPrefab;
        UnitCardDataSO unitData;
        public override void Init(CardDataSO data)
        {
            base.Init(data);

            unitData = data as UnitCardDataSO;
            
            if (unitData == null)
            {
                Debug.LogError($"{data.cardName}이 UnitCardDataSO 타입이 아님.");
            }
        }

        protected override bool TryUseCard(RaycastHit hit)
        {
            if (unitData == null || unitData.unitDataSO == null) return false;

            HexTile tile = hit.transform.GetComponent<HexTile>();

            if (tile != null)
            {
                if (tile.CanReserve(null))
                {
                    UnitBase unit = Instantiate(unitPrefab, tile.transform.position, Quaternion.identity);
                    unit.Init(unitData.unitDataSO, TeamType.Ally);
                    unit.PlaceUnit(tile);

                    UnitManager.Instance.RegisterUnit(unit);
                    UnitUIManager.Instance.Create(unit);

                    Debug.Log($"{unit.name} 배치 성공");
                    return true;
                }
                return false;
            }
            return false;
        }

    }
}
