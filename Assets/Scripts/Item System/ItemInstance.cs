using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace Item
{
    /// <summary>
    /// Item의 런타임 인스턴스
    /// </summary>
    public class ItemInstance
    {
        public ItemSO _itemData { get; private set; }       // 아이템의 정적인 정보
        public UnitBase _unit { get; private set; }         // 아이템을 보유중인 유닛
        public ItemEffectBase _effect { get; private set; }    // 아이템 부가 효과

        
        public ItemInstance(ItemSO itemData)
        {
            _itemData = itemData;
        }

        /// <summary>
        /// 아이템 장착
        /// </summary>
        /// <param name="unit"></param>
        /// <returns>장착 성공 여부</returns>
        public bool TryEquip(UnitBase unit)
        {
            if (unit == null) return false;

            if (_unit != null) return false;

            _unit = unit;


            // 스텟 유닛에 적용
            // _unit.스텟 추가();

            // 아이템의 부가 효과가 있는 경우 효과의 런타임 객체 생성
            if (_itemData.effect != null)
            {
                // 효과 생성 과정에서 문제가 생기는 경우 추척이 힘듦으로 try-catch
                try
                {
                    _effect = _itemData.effect.GetItemEffect(unit);     // 아이템 부가 효과의 런타임 객체 생성

                    if (_effect == null || !_effect.TryEquip())
                    {
                        Unequip();
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Item effect equip failed: {_itemData.name}\n{e}");
                    Unequip();
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        ///  아이템 제거 시 호출
        /// </summary>
        public void Unequip()
        {
            // 스탯 제거

            // 유닛 제거
            _unit = null;

            // 아이템 효과 제거
            if (_effect != null)
            {
                _effect.Unequip();
                _effect = null;
            }
        }

        void OnDestroy()
        {
            Unequip();
        }
    }
}
