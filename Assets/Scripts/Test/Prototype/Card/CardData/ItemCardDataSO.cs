using Item;
using UnityEngine;

namespace Prototype.Card.Item
{
    [CreateAssetMenu(fileName = "Card_Item", menuName = "CardDataSO/ItemCardDataSO")]
    public class ItemCardDataSO : CardDataSO
    {
        public ItemSO itemSO;

        public override CardType CardType => CardType.Item;
    }
}

