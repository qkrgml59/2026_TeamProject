using Prototype.Card.Item;
using System.Collections.Generic;
using UnityEngine;
using Utilitys;

namespace Item
{
    public class ItemManager : SingletonMonoBehaviour<ItemManager>
    {
        [Header ("아이템 조합법 리스트")]
        public List<ItemRecipeSO> recipes = new List<ItemRecipeSO>();

        public ItemCardDataSO GetRecipeResult(ItemSO itemA, ItemSO itemB)
        {
            foreach (ItemRecipeSO recipe in recipes)
            {
                if (recipe.IsMach(itemA, itemB))
                {
                    return recipe.resultCard;       // 조합 성공
                }
            }
            return null;        // 조합 법 없음
        }
    }
}
