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

    public void BattleStart()
    {
        OnBattleStart?.Invoke();
    }

    public void BattleEnd()
    {
        OnBattleEnd?.Invoke();
    }
}
