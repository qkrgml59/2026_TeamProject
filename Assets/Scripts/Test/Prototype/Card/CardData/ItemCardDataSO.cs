using Item;
using UnityEngine;

namespace Prototype.Card.Item
{
    [CreateAssetMenu(fileName = "CardDataSO", menuName = "CardDataSO/ItemCardDataSO")]
    public class ItemCardDataSO : CardDataSO
    {
        public ItemSO itemSO;
    }
}

