using Item;
using Prototype.Card.Item;
using Prototype.Grid;
using Prototype.UI;
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
            if (item == null || item.itemSO == null) return false;

            // Ray 맞은곳에서 HexTile컴포넌트 찾기
            HexTile tile = hit.transform.GetComponent<HexTile>();
            UnitBase targetUnit = tile != null && tile.OccupantUnit != null ? tile.OccupantUnit : hit.transform.GetComponent<UnitBase>();             

            if (targetUnit != null)
            {
                if (targetUnit.team != TeamType.Ally) return false;

                foreach (ItemBase equippedItem in targetUnit.EquippedItems)
                {
                    ItemCardDataSO resultRecipe = ItemManager.Instance.GetRecipeResult(item.itemSO, equippedItem.itemData);             // ItemManager에서 조합법 확인

                    if (resultRecipe != null && resultRecipe.itemSO != null)
                    {
                        // 장착중인 DefaultItem해제 및 제거
                        targetUnit.UnequipItem(equippedItem);

                        // 조합 아이템 생성
                        ItemBase combinedInstance = new ItemBase(resultRecipe.itemSO);

                        // 조합 아이템 장착
                        targetUnit.TryEquippedItem(combinedInstance);

                        Debug.Log($"아이템 조합 성공! {resultRecipe.itemSO.itemName}");
                        return true;
                    }
                }

                if (targetUnit.CanEquipItem(TeamType.Ally))
                {
                    ItemBase itemInstance = new ItemBase(item.itemSO);

                    return targetUnit.TryEquippedItem(itemInstance);
                }
            }

            //유닛이 없을 때
            return false;
        }
    }
}

