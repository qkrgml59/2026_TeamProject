using Item;
using Prototype.Card.Item;
using Prototype.Grid;
using Unit;
using UnityEngine;

namespace Prototype.Card
{
    public class ItemCard : CardBase
    {
        ItemCardDataSO item;

        public override void Init(CardDataSO data)
        {
            base.Init(data);

            item = data as ItemCardDataSO;

            if (item == null)
            {
                Debug.LogError($"{data.cardName}이 ItemCardDataSO 타입이 아님.");
            }
        }

        protected override bool TryUseCard(RaycastHit hit)
        {
            if (item == null || item.itemSO == null || item.itemSO.itemPrefab == null) return false;

            // Ray 맞은곳에서 HexTile컴포넌트 찾기
            HexTile tile = hit.transform.GetComponent<HexTile>();
            UnitBase targetUnit = null;             

            if (tile != null && tile.OccupantUnit != null)
            {
                targetUnit = tile.OccupantUnit;             // 타겟 유닛을 타일 속 유닛으로 지정
            }
            else                    // 타일이 아닌 유닛을 Ray로 클릭 했을 시 예외처리
            {
                targetUnit = hit.transform.GetComponent<UnitBase>();
            }

            if (targetUnit != null)
            {
                if (targetUnit.team != TeamType.Ally) return false;               // 아군일 때만 아이템을 줄 수 있게 예외처리

                if (targetUnit.EquippedItems.Count >= 3)                          // 장착 아이템이 3개 이상일 때 예외처리
                {
                    Debug.LogError("아이템은 3개까지만 장착할 수 있습니다.");
                    return false;
                }
                
                ItemBase itemInstance = Instantiate(item.itemSO.itemPrefab, targetUnit.transform.position, Quaternion.identity);
                itemInstance.itemData = item.itemSO;

                bool isEquipped = itemInstance.TryEquip(targetUnit);

                if (!isEquipped)        // 장착 실패시 생성했던 오브젝트 파괴.
                {
                    Destroy(itemInstance.gameObject);
                    return false;
                }

                return true;
            }

            //유닛이 없을 때
            return false;
        }
    }
}

