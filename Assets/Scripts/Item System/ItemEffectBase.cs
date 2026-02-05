using UnityEngine;


namespace Item
{
    /// <summary>
    /// 아이템의 추가 효과의 런타임 인스턴스
    /// </summary>
    public abstract class ItemEffectBase
    {
        protected UnitBase _unit { get; private set; }          // 아이템 장착 유닛
        protected bool IsActive { get; private set; } = false;  // 아이템의 활성화 여부

        private bool _subscribed;                               // 아이템 효과 장착 여부(이벤트 구독 여부)


        public ItemEffectBase(UnitBase unit)
        {
            _unit = unit;
        }

        /// <summary>
        /// 아이템 추가 효과 장착
        /// </summary>
        /// <returns>장착 성공 여부</returns>
        public bool TryEquip()
        {
            if (_subscribed) return false;
            _subscribed = true;

            // 아래의 전투 준비, 시작, 종료 이벤트 등록

            // 아이템 고유 이벤트 등록
            OnRegisterEvents();

            return true;
        }

        /// <summary>
        /// 아이템 추가 효과의 해제
        /// </summary>
        public void Unequip()
        {
            if (!_subscribed) return;
            _subscribed = false;

            // 아래의 전투 준비, 시작, 종료 이벤트 해제

            // 아이템 고유 이벤트 해제
            OnUnregisterEvents();
        }


        #region 아이템 공통 이벤트
        /// <summary>
        /// 전투 준비 시 호출
        /// </summary>
        public virtual void OnPrepareBattle() { }

        /// <summary>
        /// 전투 시작 시 호출
        /// (아이템 효과 활성화)
        /// </summary>
        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
            OnActivate();
        }

        /// <summary>
        /// 전투 종료 시 호출
        /// (아이템 효과 비활성화)
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
            OnDeactivate();
        }

        /// <summary>
        /// 아이템 효과 활성화
        /// </summary>
        protected virtual void OnActivate() { }

        /// <summary>
        /// 아이템 효과 비활성화
        /// </summary>
        protected virtual void OnDeactivate() { }

        #endregion

        #region 추가 효과 이벤트

        /// <summary>
        /// 아이템별 고유한 이벤트 등록 시 호출
        /// </summary>
        protected abstract void OnRegisterEvents();

        /// <summary>
        /// 아이템별 고유한 이벤트 해제 시 호출
        /// </summary>
        protected abstract void OnUnregisterEvents();

        #endregion
    }
}
