
/// <summary>
/// 기물 FSM에서 사용되는 모든 상태(State)가 반드시 구현해야 하는 인터페이스
/// </summary>
public interface IUnitState
{
    //상태에 처음 진입했을 때 1회 호출
    void StateEnter();

    //매 프레임 호출
    //행동 판단, 상태 전환 조건 체크
    void StateUpdate();

    //이동 물리 연산 등에 호출
    void StateFixedUpdate();

    //상태를 빠져나갈 때 호출
    void StateExit();
}
