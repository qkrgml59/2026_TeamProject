using Prototype.Grid;
using Stat;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Unit
{
    public enum TeamType
    {
        Null,
        Ally,
        Enemy
    }

    public class UnitBase : MonoBehaviour, IHealthReceiver
    {
        [Header("유닛 정보")]
        public TeamType team = TeamType.Ally;

        [Header("스텟 정보")]
        [SerializeField] private UnitStatSO unitStatData;
        public StatSet statSet { get; private set; }
        [SerializeField] private float currentHp = 0;

        [Header("FSM 정보")]
        [SerializeField] private UnitStateType currentUnitState = UnitStateType.Null;
        public UnitStateType CurFSM => currentUnitState;

        private IUnitState currentFSM;
        private Dictionary<UnitStateType, IUnitState> states;

        // 타겟 정보
        public UnitBase targetUnit { get; private set; }

        // 타일 정보
        public HexTile currentTile { get; private set; }
        //public HexTile nextTile { get; private set; }

        // 길찾기 경로
        public readonly List<HexTile> path = new List<HexTile>();
        public int curPathIndex { get; private set; } = 0;


    #region FSM
        public void ChangeUnitState(UnitStateType targetState)
        {
            if(states.TryGetValue(targetState, out IUnitState newState))
            {
                Debug.Log($"[{gameObject.name}] {currentUnitState} -> {targetState} 상태 변경", this);
                currentUnitState = targetState;

                // 기존 상태 종료
                currentFSM?.StateExit();

                // 새로운 상태 진입
                currentFSM = newState;
                currentFSM.StateEnter();
            }
        }

        void FSMInit()
        {
            states = new()
            {
                { UnitStateType.Idle, new IdleState(this)},
                { UnitStateType.Think, new ThinkState(this)},
                { UnitStateType.Move, new MoveState(this)},
                { UnitStateType.Attack, new AttackState(this)},
                { UnitStateType.Dead, new DeadState(this)}
            };
        }
        #endregion

        #region Physics

        // 컴포넌트
        Rigidbody _rigidbody;

        public void UpdatePathMovement()
        {
            EnterTile(path[curPathIndex]);

            if (path.Count == 0 || curPathIndex >= path.Count)
            {
                ClaerPath();
                ChangeUnitState(UnitStateType.Think);
                return;
            }

            MoveToTile(path[curPathIndex]);

            // 목표 타일에 가까워졌다면
            if(Vector3.Distance(transform.position, path[curPathIndex].transform.position) < 0.1f)
            {
                curPathIndex++;

                ChangeUnitState(UnitStateType.Think);
            }
        }

        public void MoveToTile(HexTile targetTile)
        {
            Vector3 dir = targetTile.transform.position - transform.position;
            MoveInDirection(dir);
        }

        public void MoveInDirection(Vector3 dir)
        {
            _rigidbody.linearVelocity = dir.normalized * statSet.MoveSpeed.Value;
        }
        
        public void RigidInit()
        {
            _rigidbody.linearVelocity = Vector3.zero;
        }

        public void Kinematic(bool isKinematic)
        {
            _rigidbody.isKinematic = isKinematic;
        }

        #endregion

        #region Combat

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

        public void SetTargetUnit(UnitBase newTarget)
        {
            if(newTarget == null) return;
            Debug.Log($"[{name}] 공격 대상 변경 ({newTarget.transform.name})");
            targetUnit = newTarget;

            // 이동 경로 초기화
            ClaerPath();
        }

        #endregion

        #region Event Listener
        void OnRoundStart()
        {
            // 아이템 등의 효과 초기화(또는 재적용)
            currentHp = statSet.MaxHp.Value;
            ChangeUnitState(UnitStateType.Idle);
        }

        void OnRoundEnd()
        {
            // 아이템 등 효과 제거
            ChangeUnitState(UnitStateType.Idle);
        }

        void OnBattleStart()
        {
            ChangeUnitState(UnitStateType.Think);
        }

        void OnBattleEnd()
        {

        }

        #endregion

        #region Unity Method
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            FSMInit();

            if(unitStatData != null)
                statSet = new StatSet(unitStatData);
        }

        private void Start()
        {
            BattleManager.Instance.OnRoundStart += OnRoundStart;
            BattleManager.Instance.OnRoundEnd += OnRoundEnd;
            BattleManager.Instance.OnBattleStart += OnBattleStart;
            BattleManager.Instance.OnBattleEnd += OnBattleEnd;

            // 아이템 등의 효과 초기화(또는 재적용)
            currentHp = statSet.MaxHp.Value;
            ChangeUnitState(UnitStateType.Idle);
        }

        private void Update()
        {
            //if(BattleManager.Instance.currentBattleState == BattleState.Combat)
                currentFSM?.StateUpdate();
        }

        private void FixedUpdate()
        {
            //if (BattleManager.Instance.currentBattleState == BattleState.Combat)
                currentFSM?.StateFixedUpdate();
        }

        private void OnEnable()
        {
            // 활성 상태에 따른 이벤트도 적용할 것인지 확인
        }

        private void OnDisable()
        {

        }

        private void OnDestroy()
        {
            BattleManager.Instance.OnRoundStart -= OnRoundStart;
            BattleManager.Instance.OnRoundEnd -= OnRoundEnd;
            BattleManager.Instance.OnBattleStart -= OnBattleStart;
            BattleManager.Instance.OnBattleEnd -= OnBattleEnd;
        }
        #endregion

        #region Search Method
        public UnitBase GetNearestEnemy()
        {
            var enemies = UnitManager.Instance.GetAliveEnemies(team);

            UnitBase nearest = null;
            int minDistance = int.MaxValue;

            foreach (var enemy in enemies)
            {
                int distance = HexMath.Distance(currentTile.offset, enemy.currentTile.offset);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = enemy;
                }
            }

            return nearest;
        }

        public bool TrySetPath()
        {
            // 기존 경로 초기화
            ClaerPath();

            if (currentTile == null) return false;
            if (targetUnit == null) return false;
            if (targetUnit.currentTile == null) return false;

            return GridManager.Instance.pathfinder.TryGetPath(currentTile, targetUnit.currentTile, path);
        }

        /// <summary>
        /// 현재 경로가 아직 유효한지 확인
        /// </summary>
        public bool IsPathStillValid()
        {
            if (path.Count == 0) return false;
            if (path.Count <= curPathIndex) return false;

            // 도착 타일에 해당 유닛이 있는지 확인
            if (path[^1].OccupantUnit != targetUnit)
                return false;

            return GridManager.Instance.pathfinder.IsPathStillValid(path, curPathIndex);
        }

        public void ClaerPath()
        {
            path.Clear();
            curPathIndex = 0;
        }

        #endregion

        public void EnterTile(HexTile nextTile)
        {
            currentTile?.ExitTile(this);
            nextTile.EnterTile(this);
            currentTile = nextTile;
        }

        /// <summary>
        /// 자신과 타겟이 동일한 팀 인지 반환
        /// </summary>
        public bool IsSameTeam(TeamType team)
        {
            return this.team == team;
        }    
    }
}
