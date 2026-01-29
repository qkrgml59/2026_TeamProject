using System.Xml;
using UnityEngine;

/// <summary>
/// 모든 기물(Unit)이 상속받는 공통 베이스 클래스
/// </summary>
public abstract class UnitBase : MonoBehaviour
{
    //현재 유닛이 수행중인 상태
    protected IUnitState currentState;

    [Header("Team/HP")]
    protected int teamId = 0;      //팀 이름 Ex) 아군 0, 적 1
    public int TeamId => teamId;
    public bool IsDead => maxHp <= 0;

    //공통 스텟
    [Header("Unit Stats")]
    public int maxHp;                //최대 체력
    public int currentHp;            //현재 체력
    public float AttackDamage;       //공격력
    public float AbilityPower;       //주문력
    public float AttackSpeed;        //공격 속도
    public int attackRange;          //공격 범위 (사거리)
    public float moveSpeed;          //이동 속도
    public float Defense;            //방어력
    public float attackCooldown;     //공격 쿨타임

    //현재 공격 또는 추적 중인 대상
    protected UnitBase currentTarget;

    //매 프레임 현재 상태의 Update 실행
    protected virtual void Update()
    {
        currentState?.StateUpdate();
    }


    //FixedUpdate 타이밍에 상태의 Fixedupdate실행
    protected virtual void FixedUpdate()
    {
        currentState?.StateFixedUpdate();
    }


    /// <summary>
    /// 상태 전환 함수
    /// - Exit -> Enter 순서 보장
    /// - 외부에서는 반드시 이 함수를 통해 상태 변경
    /// </summary>
    
    public void ChangeState(IUnitState newState)
    {
        if (currentState == newState) return;

        currentState?.StateExit();
        currentState = newState;
        currentState?.StateEnter();
    }


    // ========
    // 공통 행동
    // ========

 

  

}
