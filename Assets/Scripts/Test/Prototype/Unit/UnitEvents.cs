using Prototype.Unit;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Prototype.Unit
{

    /// <summary>
    /// 유닛을 주체로 하는 이벤트 모음 클래스
    /// </summary>
    [Serializable]
    public class UnitEvents
    {
        [Header("공격 이벤트")]
        public UnityEvent<UnitBase> OnNormalAttack;

        [Header("체력 관련 이벤트")]
        public UnityEvent<float, float> OnHpChanged;        // TODO: 이벤트 데이터 구조체 추가

        [Header("사망")]
        public UnityEvent<UnitBase> OnDead;

        [Header("오브젝트 파괴")]
        public UnityEvent<UnitBase> OnDestroyedUnit;

        [Header("FSM 변경")]
        public UnityEvent<UnitBase, UnitStateType> OnUnitFSMChanged;
    }
}
