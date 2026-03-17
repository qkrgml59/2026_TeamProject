using Prototype.Unit;
using System.Collections;
using System.Reflection;
using UnityEngine;
namespace Skill
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


        #endregion

        #region == Getter===

        public string SkillName => skillName;
        public Sprite Icon => icon;
        public int Cost => cost;
        public bool IsUsing => isUsing;

        public virtual string GetDescription()
        {
            return description;
        }

        #endregion

        #region === 스킬 흐름 ===

        ///<summary>
        ///스킬 사용 시작
        /// </summary>
        
        public virtual void Use(UnitBase _owner)
        {
            if (isUsing) return;
            isUsing = true;

            owner = _owner;

            OnStart();
        }

        public virtual void StopSkill()
        {
            if (!isUsing) return;
            isUsing = false;

            OnEnd();

            owner = null;
            
        }    

        #endregion

        void Update()
        {
            if (!isUsing) return;
            OnUsing();
        }


        #region ===  단계별 함수 ===

        protected abstract void OnStart();

        protected abstract void OnUsing();

        protected abstract void OnEnd();
 

        #endregion
    }
}

