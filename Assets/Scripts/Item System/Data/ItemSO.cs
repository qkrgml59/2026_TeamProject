using System.Collections.Generic;
using UnityEngine;
using Stat;

namespace Item
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "ItemSO/Default")]
    public class ItemSO : ScriptableObject
    {
        [Header("아이템 아이콘")] public Sprite icon;
        [Header("아이템 이름")] public string itemName;
        [Header("아이템 설명")][TextArea] public string itemDescription;

        [Header("추가 스텟")] public List<StatModifier> modifiers = new List<StatModifier>();
        [Header("특수 효과 모듈")] public List<ItemEffectSO> effectModules = new List<ItemEffectSO>();       // 모듈로 사용하는 이유 : 다른 아이템의 이펙트효과에서 재활용할 수 있음.
    }
}
