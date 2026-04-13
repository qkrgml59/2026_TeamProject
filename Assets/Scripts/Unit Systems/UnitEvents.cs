using Item;
using System;
using System.Collections.Generic;
using Unit;
using UnityEngine;
using UnityEngine.Events;
using StatSystem;

namespace Unit
{

    /// <summary>
    /// 유닛을 주체로 하는 이벤트 모음 클래스
    /// </summary>
    [Serializable]
    public class UnitEvents
    {
        //[Header("기본 공격 이벤트")]
        //public UnityEvent<UnitBase> OnNormalAttack;                     // 기본 공격 시작


        /// <summary>
        /// 기본 공격 적중 시 (데미지 정보, 적중 유닛)
        /// </summary>
        [Header("기본 공격 적중")]
        public UnityEvent<DamageInfo, UnitBase> OnNormalAttackHit;

        /// <summary>
        /// 피격자 기준 피격 당함 이벤트
        /// </summary>
        [Header("공격 피격 이벤트")]
        public UnityEvent<HitInfo> OnHit;

        /// <summary>
        /// 공격자 기준 공격 성공 이벤트
        /// </summary>
        [Header("공격 성공 이벤트")]
        public UnityEvent<HitInfo> OnDealtHit;

        [Header("체력 관련 이벤트")]
        public UnityEvent<HealthInfo> OnHpChanged;

        [Header("아이템 이벤트")]
        /// <summary>
        /// 아이템 리스트에 변경이 있을 시
        /// </summary>
        public UnityEvent<List<ItemBase>> OnItemChanged;

        [Header("사망")]
        public UnityEvent<UnitBase> OnDead;

        [Header("오브젝트 파괴")]
        public UnityEvent<UnitBase> OnDestroyedUnit;

        [Header("FSM 변경")]
        public UnityEvent<UnitBase, UnitStateType> OnUnitFSMChanged;
    }
}
