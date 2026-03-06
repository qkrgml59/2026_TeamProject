using Stat;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Unit
{
    public enum TeamType
    {
        Ally,
        Enemy
    }

    public class UnitBase : MonoBehaviour, IHealthReceiver
    {
        [Header("유닛 정보")]
        private TeamType team = TeamType.Ally;

        [Header("스텟 정보")]
        [SerializeField] private UnitStatSO unitStatData;
        public StatSet statSet { get; private set; }
        private float currentHp = 0;

        [Header("FSM 정보")]
        [SerializeField] private UnitStateType currentUnitState = UnitStateType.Null;
        private IUnitState currentFSM;
        private Dictionary<UnitStateType, IUnitState> states;

        [Header("타겟 정보")]
        [SerializeField] private UnitBase targetUnit;


        #region FSM
        public void ChangeUnitState(UnitStateType targetState)
        {
            if(states.TryGetValue(targetState, out IUnitState newState))
            {
                // 기존 상태 종료
                currentFSM?.StateExit();

                // 새로운 상태 진입
                currentFSM = newState;
                currentFSM.StateEnter();

                currentUnitState = targetState;
                Debug.Log($"[{gameObject.name}] {currentUnitState} -> {targetState} 상태 변경", this);
            }
        }

        void FSMInit()
        {
            states = new()
            {
                { UnitStateType.Idle, new IdleState(this)},
                { UnitStateType.Move, new MoveState(this)},
                { UnitStateType.Attack, new AttackState(this)},
                { UnitStateType.Dead, new DeadState(this)}
            };
        }
        #endregion

        public virtual void ApplyDamage(float amount)
        {
            if (amount < 0) return;

            // 방어력, 피해 감소 등 계산
            // 프로토타입에서는 생략

            if (currentHp - amount < 0)
                amount = currentHp;

            currentHp -= amount;

            Debug.Log($"[{gameObject.name}] -{amount} 데미지", this);
        }

        public void ApplyHeal(float amount)
        {
            if (amount < 0) return;

            // 회복 증가 등 생략

            // 초과 회복 처리
            if (currentHp + amount > statSet.MaxHp.Value)
                amount = statSet.MaxHp.Value - currentHp;

            currentHp += amount;

            Debug.Log($"[{gameObject.name}] +{amount} 회복", this);
        }


        #region Event Listener
        void OnRoundStart()
        {
            // 아이템 등의 효과 초기화(또는 재적용)
            currentHp = statSet.MaxHp.Value;
        }

        void OnRoundEnd()
        {
            // 아이템 등 효과 제거
        }

        #endregion

        #region Unity Method
        private void Awake()
        {
            FSMInit();

            if(unitStatData != null)
                statSet.SetStatByStar(unitStatData, 0);
        }

        private void Start()
        {
            BattleManager.Instance.OnRoundStart += OnRoundStart;
            BattleManager.Instance.OnRoundEnd += OnRoundEnd;
        }

        private void Update()
        {
            
        }

        private void FixedUpdate()
        {
            
        }

        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }

        private void OnDestroy()
        {
            BattleManager.Instance.OnRoundStart -= OnRoundStart;
            BattleManager.Instance.OnRoundEnd -= OnRoundEnd;
        }
        #endregion


        /// <summary>
        /// 자신과 타겟이 동일한 팀 인지 반환
        /// </summary>
        public bool IsSameTeam(TeamType team)
        {
            return this.team == team;
        }    
    }
}
