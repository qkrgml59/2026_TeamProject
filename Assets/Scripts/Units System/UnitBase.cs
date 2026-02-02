using System.Xml;
using UnityEngine;

/// <summary>
/// 모든 기물(Unit)이 상속받는 공통 베이스 클래스
/// 상태 머신을 실행만 한다
/// 판단은 각 State가 담당
/// </summary>
public abstract class UnitBase : MonoBehaviour, IDamageable
{  
    //현재 유닛이 수행중인 상태
    protected IUnitState currentState;

    protected virtual void Update()
    {
        //현재 상태의 Update실행
        currentState?.StateUpdate();
    }

    protected virtual void FixedUpdate()
    {
        //현재 상태의 FixedUpdate 실행
        currentState?.StateFixedUpdate();
    }

    ///<summary>
    ///상태 전환 함수
    ///이 함수를 통해서만 상태 변경
    /// </summary>
    public void ChangeState(IUnitState newState)
    {
        if (currentState == newState) return;

        currentState?.StateExit();
        currentState = newState;
        currentState?.StateEnter();
    }


    [Header("스탯")]
    public UnitStatSO unitStatSO;           //스탯 원본 데이터
    [SerializeField] float searchRadius = 20f;
    float atkTimer;

    //스탯 묶음
    protected StatSet statSet;

    protected float currentHp;         //현재 Hp    

    // 이렇게 해도 되는지 모르겠는데 형 정답을 알려줘,,,,,,,,,,,,,,,,,,,,,,,,,,,
    //현재 체력 int값으로 하기로 했는지 float으로 하기로 했는지 기억이 안 남


    [Header("Team")]
    [SerializeField] protected int teamId = 0;                  //팀 ID  Ex) 아군 0팀, 적 1팀
    public int TeamID => teamId;

    protected IDamageable target;

    public bool IsDead => currentHp <= 0;               //현재 Hp가 0보다 작으면 사망 상태


    ///<summary>
    ///데미지 처리
    ///체력 감소
    ///사망 여부 판단
    ///상태 전환은 여기서
    /// </summary>


    //기본 이동
    protected virtual void MoveToward(Vector3 p)
    {

        Vector3 dir = (p - transform.position);
        dir.y = 0;
        if (dir.sqrMagnitude < 0.0001f) return;

        dir.Normalize();
        transform.position += dir * statSet.MoveSpeed().Value * Time.deltaTime;
        transform.forward = Vector3.Lerp(transform.forward, dir, 15f * Time.deltaTime);
    }

    /// <summary>
    /// 공격 실행 명령
    /// </summary>
    protected virtual void DoAttack(IDamageable target)
    {
        OnAttack(target);
    
    }

    /// <summary>
    /// 공격 강제 구현
    /// </summary>
    protected abstract void OnAttack(IDamageable t);

    ///<summary>
    ///데미지 처리 
    ///탱커/방어막은 override
    /// </summary>
    public virtual void TakeDamage(int amount)
    {
        if (IsDead) return;
        currentHp -= amount;
        DamageText.Spawn(amount, transform.position + Vector3.up * 1.6f);

        if (currentHp <= 0) Destroy(gameObject);
    }

    /// <summary>
    /// 적(타겟) 탐지
    /// 같은 팀 제외, 가장 가까운 적 선택
    /// </summary>
    /// <returns></returns>
    IDamageable FindNearestEnemy()
    {
        // 레지스트리/매니저 없이 Physics로 “주변에서 찾기”
        Collider[] hits = Physics.OverlapSphere(transform.position, searchRadius);
        float best = float.PositiveInfinity;
        IDamageable bestTarget = null;

        foreach (var h in hits)
        {
            var d = h.GetComponentInParent<IDamageable>();
            if (d == null || d.IsDead || d.TeamID == TeamID) continue;

            float dist = (h.transform.position - transform.position).sqrMagnitude;
            if (dist < best) { best = dist; bestTarget = d; }
        }

        return bestTarget;
    }

}
