using UnityEngine;

namespace Item
{
    public abstract class ItemEffectSO : ScriptableObject
    {
        /// <summary>
        /// 아이템 효과의 런타임용 인스턴스 생성 및 반환
        /// </summary>
        /// <returns>실제 아이템 효과 인스턴스</returns>
        public abstract ItemEffectBase GetItemEffect(UnitBase unit);

        /// <summary>
        /// 아이템 효과의 설명 문자열을 반환
        /// </summary>
        /// <returns>효과 설명 String</returns>
        public abstract string GetDescription();
    }
}
