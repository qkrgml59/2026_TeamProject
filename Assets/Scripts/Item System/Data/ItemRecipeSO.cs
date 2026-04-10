using Prototype.Card.Item;
using UnityEngine;

namespace Item
{
    [CreateAssetMenu(fileName = "New ItemRecipe", menuName = "ItemSO/ItemRecipe")]
    public class ItemRecipeSO : ScriptableObject
    {
        [Header ("재료 아이템")]
        public ItemSO itemA;
        public ItemSO itemB;

        [Header("완성 아이템")]
        public ItemCardDataSO resultCard;

        public bool IsMach(ItemSO _itemA, ItemSO _itemB)
        {
            return (itemA == _itemA && itemB == _itemB) ||
                   (itemA == _itemB && itemB == _itemA);
        }
    }
}
