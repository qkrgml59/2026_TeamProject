using Prototype.Grid;
using Stat;
using System.Collections.Generic;
using UnityEngine;
using Unit.Skill;
using Item;


namespace Unit
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
        public TeamType team { get; private set; } = TeamType.Ally;
        [SerializeField] private Texture2D dummyImage;
        [SerializeField] private Renderer quadRenderer;
        private MaterialPropertyBlock mpb;
        [SerializeField] private UnitDataSO unitDataSO;
        private UnitStatSO unitStatData;

        public StatSet statSet { get; private set; }            //스텟 수정할 때
        public float currentHp { get; private set; } = 0;

        [Header("아이템 정보")]
        public List<ItemBase> EquippedItems { get; private set; } = new List<ItemBase>();           // 장착 중인 아이템 리스트

        [Header("FSM 정보")]
        [SerializeField] private UnitStateType currentUnitState = UnitStateType.Null;
        public UnitStateType CurFSM => currentUnitState;

        private IUnitState currentFSM;
        private Dictionary<UnitStateType, IUnitState> states;

        [Header("기본 공격")]
        public SkillBase normalAttack;

        [Header("스킬 공격")]
        public SkillBase skill;

        [Header("유닛 이벤트")]
        public UnitEvents unitEvents;

        // 타겟 정보
        public UnitBase targetUnit { get; private set; }

        // 위치 정보
        public Vector2Int offset => currentTile ? currentTile.Offset : Vector2Int.zero;        // 현재 타일 기준 오프셋 보내기
        public HexTile currentTile { get; private set; }
        public HexTile reservedTile { get; private set; }
        // 길찾기 경로
        public readonly List<HexTile> path = new List<HexTile>();
        public int curPathIndex { get; private set; } = 0;

        private float tileSpace = 1;

        // 컴포넌트
        Rigidbody _rigidbody;

        // 초기화 정보
        HexTile startTile;

        // 자원 정보
        private float attackResourceRegen = 10;      // 기본 공격 회복량 TODO : 캐릭터 정보로 넘기기
        public float currentResource { get; private set; } = 0;

        /// <summary>
        /// 유닛의 배치 (초기 위치 설정)
        /// </summary>
        /// <param name="tile"></param>
        public void PlaceUnit(HexTile tile)
        {
            reservedTile = null;
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
            else
            {
                Debug.LogWarning($"{targetState} 상태가 정의되어 있지 않습니다.");
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
                { UnitStateType.SKill, new SkillState(this)},
                { UnitStateType.Stun, new StunState(this)},
                { UnitStateType.Dead, new DeadState(this)}
            };
        }
        #endregion

        #region Physics
        public void MoveToTile(HexTile targetTile, float speed)
        {
            Vector3 dir = targetTile.transform.position - transform.position;
            MoveInDirection(dir, speed);
        }

        public void MoveInDirection(Vector3 dir, float speed)
        {   
            // 모든 이동은 타일 크기 기준으로 이동 (속도 1 = 1 Tile/s)
            _rigidbody.linearVelocity = dir.normalized * speed * tileSpace;
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
            MoveToTile(reservedTile, statSet.MoveSpeed.Value);

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
            ClearPath();

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
                ClearPath();
                ChangeUnitState(UnitStateType.Think);
                return;
            }


            if(path[curPathIndex].TryReserve(this))
            {
                reservedTile = path[curPathIndex];
            }
            else
            {
                ClearPath();
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

        public void ClearPath()
        {
            path.Clear();
            curPathIndex = 0;
        }
        #endregion

        #region Combat

        public void NormalAttack()
        {
            if(normalAttack == null)
            {
                ChangeUnitState(UnitStateType.Think);
                Debug.LogWarning($"[{name}] 유닛의 기본 공격을 설정하지 않았습니다.");
                return;
            }

            normalAttack.Use();
            unitEvents.OnNormalAttack?.Invoke(this);
        }

        public void UseSkill()
        {
            if(skill == null)
            {
                ChangeUnitState(UnitStateType.Think);
                Debug.LogWarning($"[{name}] 유닛은 스킬이 없습니다.");
                return;
            }

            skill.Use();
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

            if(info.isCritical) Debug.Log($"[{gameObject.name}] -{damage} 치명타!! (공격 유닛 : {info.caster})", this);
            else Debug.Log($"[{gameObject.name}] -{damage} 데미지 (공격 유닛 : {info.caster})", this);

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

        public bool IsSkillReady()
        {
            return skill != null
                && currentResource >= skill.Cost;
                // 침묵 상태 같은건 없는지
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

            if (skill != null && currentResource >= skill.Cost)
            {
                currentResource = skill.Cost;

                if (CurFSM != UnitStateType.Move || CurFSM != UnitStateType.Stun ||
                    CurFSM != UnitStateType.Dead)
                    ChangeUnitState(UnitStateType.SKill);
            }

            Debug.Log($"[{name}] 현재 자원 {currentResource}");
        }

        public void UseResource(float amount)
        {
            if (amount <= 0) return;

            // 0 이하로는 감소 하지 않도록
            currentResource = Mathf.Max(currentResource - amount, 0);
            Debug.Log($"[{name}] 현재 자원 {currentResource}");
        }

        #endregion

        #region 스탯 관리
        /// <summary>
        /// 유닛의 스탯을 증가 시킬 때 사용.
        /// </summary>
        /// <param name="type"> 변경할 스탯 타입 </param>
        /// <param name="source"> 스텟을 부여하는 주체(아이템, 시너지 등)</param>
        /// <param name="modifier"> 증가량 </param>
        public void AddStatModifier(StatType type, object source, StatModifier modifier)
        {
            Stat.Stat stat = statSet.Get(type);

            if (stat != null)
            {
                stat.AddModifier(source, modifier);
            }
        }

        public void RemoveStatModifier(object source)
        {
            foreach (StatType type in System.Enum.GetValues(typeof(StatType)))
            {
                statSet.Get(type)?.RemoveModifier(source);
            }
        }

        #endregion

        #region 아이템 관리
        public bool CanEquipItem(TeamType team)
        {
            return EquippedItems.Count < 3              // 아이템 개수가 3개 미만일 때
                && IsSameTeam(team);                    // 같은 팀 일때
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
            PlaceUnit(startTile);

            // 아이템 등의 효과 초기화(또는 재적용)
            currentHp = statSet.MaxHp.Value;
            unitEvents.OnHpChanged?.Invoke(currentHp, statSet.MaxHp.Value);
            currentResource = 0;

            // 스킬 초기화
            normalAttack?.Init(this);
            skill?.Init(this);

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
            FSMInit();
        }

        public void Init(UnitDataSO data, TeamType team)
        {
            this.team = team;

            if (data == null)
                Debug.LogWarning("유닛 정보를 지정하지 않은 유닛이 있습니다.", this);
            else
                unitDataSO = data;

            if(unitDataSO.statData == null)
                Debug.LogWarning("스텟 정보를 지정하지 않은 유닛이 있습니다.", this);
            else
                unitStatData = unitDataSO.statData;

            statSet = new StatSet(unitStatData);

            // 아이템 등의 효과 초기화(또는 재적용)
            currentHp = statSet.MaxHp.Value;

            // TODO : 자식 위치에 생성 -> 추적으로 변경
            // 기본 공격 & 스킬 생성
            if (unitDataSO.NormalAttack_Prefab != null)
                normalAttack = Instantiate(unitDataSO.NormalAttack_Prefab, transform.position, Quaternion.identity, transform);
            if (unitDataSO.Skill_Prefab != null)
                skill = Instantiate(unitDataSO.Skill_Prefab, transform.position, Quaternion.identity, transform);

            normalAttack?.Init(this);
            skill?.Init(this);

            // 이미지 적용
            ApplyVisual(unitDataSO.unitSprite.texture);

            // 이름 변경
            transform.name = unitDataSO.Name_EN;
        }

        private void Start()
        {
            if(GridManager.Instance != null) tileSpace = GridManager.Instance.TileSpacing;   // 타일 간격은 처음에 저장
            AddEventListener();
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
            BattleManager.Instance.OnUnitInit += OnRoundStart;
            BattleManager.Instance.OnRoundEnd += OnRoundEnd;
            BattleManager.Instance.OnBattleStart += OnBattleStart;
            BattleManager.Instance.OnBattleEnd += OnBattleEnd;

            unitEvents.OnNormalAttackHit.AddListener(OnNormalAttackHit);
        }

        public void RemoveEventListener()
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.OnUnitInit -= OnRoundStart;
                BattleManager.Instance.OnRoundEnd -= OnRoundEnd;
                BattleManager.Instance.OnBattleStart -= OnBattleStart;
                BattleManager.Instance.OnBattleEnd -= OnBattleEnd;
            }

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
            ClearPath();

            Debug.Log($"[{name}] 공격 대상 변경 ({newTarget.transform.name})");
        }
        #endregion

        #region Visual Function
        // 유닛 텍스쳐 적용
        public void ApplyVisual(Texture2D texture)
        {
            if (quadRenderer == null || dummyImage == null)
                return;

            // 텍스쳐가 없다면 더미 이미지로 띄워줌
            if(texture == null)
                texture = dummyImage;

            if (mpb == null)
                mpb = new MaterialPropertyBlock();

            quadRenderer.GetPropertyBlock(mpb);
            mpb.SetTexture("_BaseMap", texture);              
            quadRenderer.SetPropertyBlock(mpb);
        }
        #endregion

        #region Debugging

        #if UNITY_EDITOR
        private void OnValidate()
        {
            // TODO : 유닛 데이터 (스텟 데이터 X)에 있는 이미지나 애니메이터 사용하도록 변경
            if (!Application.isPlaying && dummyImage != null)
            {
                ApplyVisual(dummyImage);
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
