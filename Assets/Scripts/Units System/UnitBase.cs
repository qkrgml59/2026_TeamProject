using System.Xml;
using UnityEngine;

/// <summary>
/// 모든 기물(Unit)이 상속받는 공통 베이스 클래스
/// 상태 머신을 실행만 한다
/// 판단은 각 State가 담당
/// </summary>
public abstract class UnitBase : MonoBehaviour
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

    /// <summary>
    /// 공통 데이터
    /// </summary>

    //스탯 묶음
    protected StatSet stateSet;

    //스탯 원본 데이터
    public UnitStatSO unitStatSO;

    //현재 공격 / 추적 중인 대상
    protected UnitBase currentTarget;

    protected virtual void Start()
    {
        stateSet = new StatSet(unitStatSO);         //SO기반으로 스탯 초기화
    }

    public void SetTarget(UnitBase target)
    {
        currentTarget = target;
    }

    public bool HasTarget()
    {
        return currentTarget != null;
    }


    //우선 수정 중이빈다.

}
