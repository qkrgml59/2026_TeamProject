using System.Collections.Generic;
using UnityEngine;
using Stat;

namespace Item
{
    [CreateAssetMenu(fileName = "DefaultItem", menuName = "ItemSO/Default")]
    public class ItemSO : ScriptableObject
    {
        [Header("아이템 아이콘")] public Sprite icon;
        [Header("아이템 이름")] public string itemName;
        [Header("아이템 설명")][TextArea] public string itemDescription;
        [Header("추가 스텟")] public List<StatModifier> modifiers = new List<StatModifier>();
        [Header("아이템 부가 효과")] public ItemEffectSO effect;


        /// <summary>
        /// 아이템의 추가 스텟에 대한 문자열을 반환합니다.
        /// </summary>
        /// <returns>추가 스텟 문자열</returns>
        public string GetModifiersString()
        {
            return null;        // 아이콘 같은걸 포함해서 반환 예정
        }

        /// <summary>
        /// 아이템 부과 효과의 설명을 반환합니다.
        /// </summary>
        /// <returns>부과 효과 설명 문자열</returns>
        public string GetEffectDescription()
        {
            if (effect == null)
                return null;

            return effect.GetDescription();
        }
    }
}
