using System.Collections.Generic;
using Unit;
using UnityEngine;

namespace Stat
{
    public class StatSet
    {
        Dictionary<StatType, Stat> stats = new Dictionary<StatType, Stat>();

        //생성자 초기화
        public StatSet(UnitStatSO unitStatSO)
        {
            stats.Clear();

            // 기초 스텟 선언
            //성급에 따라 변하는 스텟 선언
            stats.Add(StatType.HealthPoint, new Stat());
            stats.Add(StatType.AttackDamage, new Stat());

            //성급 무관 고정 스텟 선언
            stats.Add(StatType.AbilityPower, new Stat(unitStatSO.AbilityPower));
            stats.Add(StatType.AttackSpeed, new Stat(unitStatSO.AttackSpeed));
            stats.Add(StatType.Defense, new Stat(unitStatSO.Defense));
            stats.Add(StatType.MagicResistance, new Stat(unitStatSO.MagicResistance));

            // 자원 스텟 선언
            stats.Add(StatType.ManaRegeneration, new Stat(unitStatSO.ManaRegeneration));

            // 특수 스텟 선언(고정값)
            stats.Add(StatType.CriticalChance, new Stat(25));
            stats.Add(StatType.CriticalDamage, new Stat(140));
            stats.Add(StatType.DamageIncrease, new Stat(0));
            stats.Add(StatType.DamageReduction, new Stat(0));
            stats.Add(StatType.Omnivamp, new Stat(0));

            stats.Add(StatType.AttackRange, new Stat(unitStatSO.Attack_Rage));     // 사거리 초기값
            stats.Add(StatType.MoveSpeed, new Stat(unitStatSO.Move_Speed));        // 이동속도 초기값

            // 1성으로 초기화
            SetStatByStar(unitStatSO, 0);
        }

        /// <summary>
        /// 유닛의 성급에 따른 기초 스텟 적용
        /// </summary>
        /// <param name="star">유닛의 성급 ( 1성 = 0 )</param>
        public void SetStatByStar(UnitStatSO unitStatSO, int star)
        {
            if (unitStatSO.statsByStart.Length <= star)
            {
                Debug.LogWarning($"{unitStatSO.name}의 ${star + 1}성 데이터가 없습니다.");
                return;
            }

            StatByStar data = unitStatSO.statsByStart[star];

            // 성급에 따른 스텍 적용
            stats[StatType.HealthPoint].SetBaseValue(data.MaxHp);
            stats[StatType.AttackDamage].SetBaseValue(data.AttackDamage);
        }

        #region Quick Method

        /// <summary>
        /// StatType으로 Stat 찾기
        /// </summary>
        public Stat Get(StatType type)
        {
            if (stats.ContainsKey(type))
                return stats[type];
            else
                return null;
        }

        // Stat 반환
        public Stat MaxHp => Get(StatType.HealthPoint);                   // 최대 체력
        public Stat AttackDamage => Get(StatType.AttackDamage);           // 공격력
        public Stat AbilityPower => Get(StatType.AbilityPower);           // 주문력
        public Stat AttackSpeed => Get(StatType.AttackSpeed);             // 공격 속도
        public Stat Defense => Get(StatType.Defense);                     // 방어력
        public Stat MagicResist => Get(StatType.MagicResistance);         // 마법 저항력
        public Stat ManaRegen => Get(StatType.ManaRegeneration);          // 마나 재생
        public Stat CritChance => Get(StatType.CriticalChance);           // 치명타 확률
        public Stat CritDamage => Get(StatType.CriticalDamage);           // 치명타 데미지
        public Stat DamageIncrease => Get(StatType.DamageIncrease);       // 가하는 피해 증가
        public Stat DamageReduction => Get(StatType.DamageReduction);     // 받는 피해 감소
        public Stat Omnivamp => Get(StatType.Omnivamp);                   // 생명력 흡수
        public Stat AttackRange => Get(StatType.AttackRange);             // 공격 사거리
        public Stat MoveSpeed => Get(StatType.MoveSpeed);                 // 이동 속도
        public Stat MaxMana => Get(StatType.MaxMana);                     // 최대 마나
        public Stat StartMana => Get(StatType.StartMana);                 // 초기 마나

        #endregion
    }

}
