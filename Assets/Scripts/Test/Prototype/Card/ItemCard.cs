using Item;
using Prototype.Card;
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
            if (item == null || item.itemSO == null) return false;

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
                if (targetUnit.team != TeamType.Ally)               // 아군일 때만 아이템을 줄 수 있게 예외처리
                {
                    return false;
                }

                ItemSO itemData = item.itemSO;

                if (itemData.effect != null)
                {
                    ItemEffectBase effectInstance = itemData.effect.GetItemEffect(targetUnit);
                    bool isEquipped = effectInstance.TryEquip();

                    if (!isEquipped)            //장착 실패 시
                    {
                        return false;
                    }
                }

                Debug.Log($"{targetUnit.name}에게 {itemData.itemName}장착 성공");
                return true;
            }

            //유닛이 없을 때
            return false;
        }

    }
}

