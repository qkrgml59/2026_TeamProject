using Prototype.Grid;
using Stat;
using System.Collections.Generic;
using UnityEngine;
using Prototype.Skill;


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
        [SerializeField] private Sprite unitImage;
        [SerializeField] private Renderer quadRenderer;
        private MaterialPropertyBlock mpb;

        [Header("스텟 정보")]
        [SerializeField] private UnitStatSO unitStatData;
        public StatSet statSet { get; private set; }
        public float currentHp { get; private set; } = 0;

        [Header("FSM 정보")]
        [SerializeField] private UnitStateType currentUnitState = UnitStateType.Null;
        public UnitStateType CurFSM => currentUnitState;

        private IUnitState currentFSM;
        private Dictionary<UnitStateType, IUnitState> states;

        [Header("기본 공격")]
        public SkillBase normalAttack;
        // 스킬 정보는 따로 저장

        [Header("유닛 이벤트")]
        public UnitEvents unitEvents;

        // 타겟 정보
        public UnitBase targetUnit { get; private set; }
        // 타일 정보
        public HexTile currentTile { get; private set; }
        public HexTile reservedTile { get; private set; }
        // 길찾기 경로
        public readonly List<HexTile> path = new List<HexTile>();
        public int curPathIndex { get; private set; } = 0;

        // 컴포넌트
        Rigidbody _rigidbody;

        // 초기화 정보
        HexTile startTile;

        // 자원 정보
        private float attackResourceRegen = 5;      // 기본 공격 회복량 TODO : 캐릭터 정보로 넘기기
        private float currentResource = 0;

        public void PlaceUnit(HexTile tile)
        {
            startTile = tile;
            transform.position = startTile.transform.position;
            EnterTile(tile);
        }

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

        #region PathFinding

        public void UpdatePathMovement()
        {
            MoveToTile(reservedTile);

            // 목표 타일에 가까워졌다면
            if(Vector3.Distance(transform.position, reservedTile.transform.position) < 0.1f)
            {
                curPathIndex++;

                EnterTile(reservedTile);
                reservedTile = null;

                ChangeUnitState(UnitStateType.Think);
            }
        }

        /// <summary>
        /// 새로운 경로 탐색
        /// </summary>
        public bool TrySetPath()
        {
            // 기존 경로 초기화
            ClaerPath();

            if (currentTile == null) return false;
            if (targetUnit == null) return false;
            if (targetUnit.currentTile == null) return false;

            return GridManager.Instance.pathfinder.TryGetPath(currentTile, targetUnit.currentTile, path, statSet.AttackRange.Value);
        }

        /// <summary>
        /// 진입 타일 설정
        /// </summary>
        public void SetReservedTile()
        {
            if (curPathIndex < 0 || curPathIndex >= path.Count)
            {
                // 경로 문제가 있는 경우
                // 경로 제거 후 상태 전환
                ClaerPath();
                ChangeUnitState(UnitStateType.Think);
                return;
            }


            if(path[curPathIndex].TryReserve(this))
            {
                reservedTile = path[curPathIndex];
            }
            else
            {
                ClaerPath();
                ChangeUnitState(UnitStateType.Think);
            }
        }

        public void EnterTile(HexTile nextTile)
        {
            currentTile?.ExitTile(this);
            nextTile?.EnterTile(this);
            currentTile = nextTile;
        }

        /// <summary>
        /// 현재 경로가 아직 유효한지 확인
        /// </summary>
        public bool IsPathStillValid()
        {
            if (path.Count == 0) return false;
            if (path.Count <= curPathIndex) return false;

            // 도착 타일에 해당 유닛이 있는지 확인
            //if (path[^1].OccupantUnit != targetUnit)
                //return false;

            return GridManager.Instance.pathfinder.IsPathStillValid(path, curPathIndex, targetUnit.currentTile, statSet.AttackRange.Value);
        }

        public void ClaerPath()
        {
            path.Clear();
            curPathIndex = 0;
        }
        #endregion

        #region Combat

        public void NormalAttack()
        {
            normalAttack?.UseSkill(this, targetUnit);
            unitEvents.OnNormalAttack?.Invoke(this);
        }

        public virtual void ApplyDamage(DamageInfo info)
        {
            float damage = info.amount;

            if (damage <= 0) return;

            if (BattleManager.Instance.currentBattleState != BattleState.Combat)
                return;

            // 방어력, 피해 감소 등 계산
            damage = DamageCalculator.CalculateFinalDamage(info, this);

            // 일단 바로 사망 처리
            if (currentHp - damage < 0)
            {
                ChangeUnitState(UnitStateType.Dead);
                return;
            }

            currentHp -= damage;

            if(info.isCritical) Debug.Log($"[{gameObject.name}] -{damage} 치명타!! (공격 유닛 : {info.source})", this);
            else Debug.Log($"[{gameObject.name}] -{damage} 데미지 (공격 유닛 : {info.source})", this);

            unitEvents.OnHpChanged?.Invoke(currentHp, statSet.MaxHp.Value);
        }

        public void ApplyHeal(HealInfo info)
        {
            float healAmount = info.amount;

            if (healAmount <= 0) return;

            if (BattleManager.Instance.currentBattleState != BattleState.Combat)
                return;

            // 회복 증가 등 생략

            // 초과 회복 처리
            if (currentHp + healAmount > statSet.MaxHp.Value)
                healAmount = statSet.MaxHp.Value - currentHp;

            currentHp += healAmount;

            Debug.Log($"[{gameObject.name}] +{healAmount} 회복 (회복 유닛 : {info.source})", this);

            unitEvents.OnHpChanged?.Invoke(currentHp, statSet.MaxHp.Value);
        }

        /// <summary>
        /// 유닛이 실제로 사망 했을 때 호출 (외부에서 직접 파괴)
        /// </summary>
        public void Die()
        {
            unitEvents.OnDestroyedUnit?.Invoke(this);     // 유닛 파괴 이벤트
            EnterTile(null);      // 위치했던 타일 제거
            RemoveEventListener();
            UnitManager.Instance.UnregisterUnit(this);
            Destroy(gameObject);
        }

        /// <summary>
        /// 자신과 타겟이 동일한 팀 인지 반환
        /// </summary>
        public bool IsSameTeam(TeamType team)
        {
            return this.team == team;
        }
        #endregion

        #region 자원 관리

        // 초당 자원 회복
        public virtual void RegenerateResource()
        {
            float amount = statSet.ManaRegen.Value * Time.deltaTime;
            RestoreResource(amount);
        }

        public void RestoreResource(float amount)
        {
            if (amount <= 0) return;

            currentResource += amount;
            Debug.Log($"[{name}] 현재 자원 {currentResource}");
        }

        #endregion

        #region CombatEvent Listner

        // 기본 공격이 적중 시
        protected virtual void OnNormalAttackHit(DamageInfo info, UnitBase hitUnit)
        {
            RestoreResource(attackResourceRegen);
        }

         // 타겟 유닛 사망 시 이벤트를 통해 호출됨
        protected virtual void OnTargetDead(UnitBase deadUnit)
        {
            deadUnit.unitEvents.OnDead.RemoveListener(OnTargetDead);

            if (targetUnit != deadUnit)
                return;

            targetUnit = null;
            ChangeUnitState(UnitStateType.Think);
        }

        #endregion

        #region GameEvent Listener
        void OnRoundStart()
        {
            transform.position = startTile.transform.position;

            // 아이템 등의 효과 초기화(또는 재적용)
            currentHp = statSet.MaxHp.Value;
            unitEvents.OnHpChanged?.Invoke(currentHp, statSet.MaxHp.Value);

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
            ChangeUnitState(UnitStateType.Idle);
        }

        #endregion

        #region Unity Method + 이벤트
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            ApplyVisual();

            FSMInit();

            if(unitStatData != null)
                statSet = new StatSet(unitStatData);
        }

        private void Start()
        {
            AddEventListener();

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
            RemoveEventListener();
        }

        public void AddEventListener()
        {
            BattleManager.Instance.OnRoundStart += OnRoundStart;
            BattleManager.Instance.OnRoundEnd += OnRoundEnd;
            BattleManager.Instance.OnBattleStart += OnBattleStart;
            BattleManager.Instance.OnBattleEnd += OnBattleEnd;

            unitEvents.OnNormalAttackHit.AddListener(OnNormalAttackHit);
        }

        public void RemoveEventListener()
        {
            BattleManager.Instance.OnRoundStart -= OnRoundStart;
            BattleManager.Instance.OnRoundEnd -= OnRoundEnd;
            BattleManager.Instance.OnBattleStart -= OnBattleStart;
            BattleManager.Instance.OnBattleEnd -= OnBattleEnd;

            unitEvents.OnNormalAttackHit.RemoveListener(OnNormalAttackHit);

            if (targetUnit != null)
                targetUnit.unitEvents.OnDead.RemoveListener(OnTargetDead);
        }
        #endregion

        #region Search Method

        public void SetTargetUnit(UnitBase newTarget)
        {
            if (newTarget == null || newTarget == targetUnit) return;

            targetUnit?.unitEvents.OnDead.RemoveListener(OnTargetDead);

            targetUnit = newTarget;
            newTarget.unitEvents.OnDead.AddListener(OnTargetDead);

            // 이동 경로 초기화
            ClaerPath();

            Debug.Log($"[{name}] 공격 대상 변경 ({newTarget.transform.name})");
        }

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



        #endregion

        #region Visual Function
        // 유닉 텍스쳐 적용
        public void ApplyVisual()
        {
            if (quadRenderer == null || unitImage == null)
                return;

            if (mpb == null)
                mpb = new MaterialPropertyBlock();

            quadRenderer.GetPropertyBlock(mpb);
            mpb.SetTexture("_BaseMap", unitImage.texture);              
            quadRenderer.SetPropertyBlock(mpb);
        }
        #endregion

        #region Debugging

        #if UNITY_EDITOR
        private void OnValidate()
        {
            // TODO : 유닛 데이터 (스텟 데이터 X)에 있는 이미지나 애니메이터 사용하도록 변경
            if (!Application.isPlaying)
            {
                ApplyVisual();
            }
        }
        #endif

        private void OnDrawGizmos()
        {
            if (path != null && path.Count > 0)
            {
                for (int i = 1; i < path.Count; i++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(path[i - 1].transform.position, path[i].transform.position);
                }
            }
        }

        #endregion
    }
}
