using Item;
using Stat;
using Unit;
using UnityEngine;

namespace Item
{
    public class DefaultItem : ItemBase
    {
        protected override void ApplyItemEffect(UnitBase unit)
        {
            if (itemData == null) return;         // ItemSO가 Null일 때의 예외처리

            if (itemData.modifiers != null)
            {
                // modifier 유닛에 적용
                foreach (StatModifier modifier in itemData.modifiers)
                {
                    unit.AddStatModifier(modifier.statType, this, modifier);                // this = 스탯 변경의 출처
                }
            }

            // TODO 특수효과(조합아이템) 장착
        }

        protected override void RemoveItemEffect(UnitBase unit)
        {
            if (itemData == null) return;         // ItemSO가 Null일 때 예외처리

            if (itemData.modifiers != null)
            {
                foreach (StatModifier modifier in itemData.modifiers)
                {
                    unit.RemoveStatModifier(this);
                }
            }
            // TODO 특수효과(조합아이템) 해제
        }
    }
}
