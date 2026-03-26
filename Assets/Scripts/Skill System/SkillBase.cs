using System.Collections;
using UnityEngine;

namespace Unit.Skill
{
    /// <summary>
    /// 모든 스킬의 부모 클래스
    /// </summary>
    public abstract class SkillBase : MonoBehaviour
    {
        #region === 데이터 ===

        [Header("기본 정보")]
        [SerializeField] protected string skillName;
        [SerializeField] protected Sprite icon;
        [SerializeField] protected string description;

        [Header("코스트")]
        [SerializeField] protected int cost;

        #endregion
    

        #region === 상태 ====

        protected bool isUsing;
        protected UnitBase owner;

        protected Coroutine skillRoutine;


        #endregion

        #region == Getter===

        public string SkillName => skillName;
        public Sprite Icon => icon;
        public int Cost => cost;

        public virtual string GetDescription()
        {
            return description;
        }

        public virtual bool CanUse()
        {
            if (isUsing) return false;

            return true;
        }

        #endregion


        #region === 초기화 ===

        public virtual void Init(UnitBase _owner)
        {
            owner = _owner;
        }

        #endregion

        #region === 스킬 흐름 ===

        ///<summary>
        ///스킬 사용 시작
        /// </summary>

        public virtual void Use()
        {
            if (isUsing) return;
            isUsing = true;

            if(owner == null)
            {
                Debug.LogWarning($"[{skillName}] 해당 스킬의 사용자가 설정되지 않았습니다.");
                return;
            }

            // 코스트 만큼 마나 소모
            owner.UseResource(cost);

            OnStart();
        }

        /// <summary>
        /// 외부에서 강제로 스킬을 종료 시킬 때
        /// </summary>
        public virtual void CancelSkill()
        {
            if (!isUsing)
                return;

            isUsing = false;
            OnCancel();
        }

        /// <summary>
        /// 스킬이 종료 되었을 때
        /// </summary>
        protected void FinishSkill()
        {
            if (!isUsing)
                return;

            isUsing = false;
            OnFinish();

            // 유닛 상태 변경
            owner.ChangeUnitState(UnitStateType.Think);
        }

        #endregion

        #region ===  단계별 함수 ===

        protected abstract void OnStart();
        protected abstract void OnCancel();
        protected abstract void OnFinish();
 

        #endregion
    }
}

