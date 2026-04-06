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
                    Stat.Stat targetStat = unit.statSet.Get(modifier.statType);

                    if (targetStat != null)
                    {
                        targetStat.AddModifier(this, modifier);         // this = 적용하려는 아이템
                    }
                }
            }

            // TODO 특수효과(조합아이템) 장착
        }

        protected override void RemoveItemEffect(UnitBase unit)
        {
            if (itemData == null) return;         // ItemSO가 Null일 때 예외처리

            if (itemData.modifiers != null)
            {
                foreach (StatModifier mod in itemData.modifiers)
                {
                    Stat.Stat targetStat = unit.statSet.Get(mod.statType);

                    if (targetStat != null)
                    {
                        // 출처(this)를 기준으로 찾아 지웁니다.
                        targetStat.RemoveModifier(this);
                    }
                }
            }
            // TODO 특수효과(조합아이템) 해제
        }
    }
}
