using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default_Item", menuName = "ItemSO/Default")]
public class ItemSO : ScriptableObject
{
    [Header("아이템 이름")] public string itemName;
    [Header("아이템 설명")] public string itemDescription;
    [Header("아이템 아이콘")] public Sprite icon;
    [Header("아이템 종류(재료, 완성, 유물 등")] public ItemType type;
    [Header("추가 스텟")] public List<StatModifier> modifiers = new List<StatModifier>();
}
