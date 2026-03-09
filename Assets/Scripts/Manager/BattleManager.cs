using System;
using Utilitys;
using UnityEngine;

public enum BattleState
{
    Prepare,            // 준비중
    Combat,             // 전투중
    RoundEnd            // 라운드 종료
}

public class BattleManager : SingletonMonoBehaviour<BattleManager>
{
    // 라운드 이벤트
    public event Action OnRoundStart;
    public event Action OnRoundEnd;

    // 전투 이벤트
    public event Action OnBattleStart;
    public event Action OnBattleEnd;

    public BattleState currentBattleState { get; private set; } = BattleState.Prepare;

    [ContextMenu("라운드 시작")]
    public void RoundStart()
    {
        OnRoundStart?.Invoke();
        currentBattleState = BattleState.Prepare;
    }

    [ContextMenu("라운드 종료")]
    public void RoundEnd()
    {
        OnRoundEnd?.Invoke();
        currentBattleState = BattleState.Prepare;
    }

    [ContextMenu("전투 시작")]
    public void BattleStart()
    {
        OnBattleStart?.Invoke();
        currentBattleState = BattleState.Combat;
    }

    [ContextMenu("전투 종료")]
    public void BattleEnd()
    {
        OnBattleEnd?.Invoke();
        currentBattleState = BattleState.RoundEnd;
    }
}
