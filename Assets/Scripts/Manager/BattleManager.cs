using Prototype.Card;
using System;
using System.Collections;
using Test.UI;
using UI;
using Unit;
using UnityEngine;
using Utilitys;

public enum BattleState
{
    Loading,            // 데이터 로딩, 시스템 준비
    Prepare,            // 준비중
    Combat,             // 전투중
    RoundEnd,           // 라운드 종료
    Reward,             // 보상 획득
}

public class BattleManager : SingletonMonoBehaviour<BattleManager>
{
    [Header("각 단계 시간 설정")]
    public float combatDuration = 30f;
    public float roundEndDuration = 3f;
    public float nextRoundDuration = 2f;

    // 라운드 이벤트
    public event Action<RoundData> OnRoundStart;
    public event Action OnUnitInit;
    public event Action OnRoundEnd;

    // 전투 이벤트
    public event Action OnBattleStart;
    public event Action OnBattleEnd;

    public BattleState currentBattleState { get; private set; } = BattleState.Prepare;

    // 임시 코루틴
    Coroutine delayRountine;
    private float duration;

    private RoundData roundData;
    

    private void Start()
    {
        BindRewardUI();

        if (delayRountine != null)
        {
            StopCoroutine(delayRountine);
            delayRountine = null;
        }

        delayRountine = StartCoroutine(DelayRountine(0.1f, () => RoundStart()));
    }

    [ContextMenu("라운드 시작")]
    public void RoundStart()
    {
        Debug.Log("[Battle Manager] 라운드 시작");
        if (StageManager.Instance == null || StageManager.Instance.CurrentRound == null)
        {
            Debug.LogError("라운드 데이터가 없습니다.");
            return;
        }
        
        OnRoundStart?.Invoke(StageManager.Instance.CurrentRound);
        OnUnitInit?.Invoke();
        currentBattleState = BattleState.Prepare;
    }

    [ContextMenu("전투 시작")]
    public void BattleStart()
    {
        Debug.Log("[Battle Manager] 전투 시작");
        OnBattleStart?.Invoke();
        currentBattleState = BattleState.Combat;

        if (delayRountine != null)
        {
            StopCoroutine(delayRountine);
            delayRountine = null;
        }

        delayRountine = StartCoroutine(DelayRountine(combatDuration, () => BattleEnd()));


        // 전투 시작 직후 종료 가능 여부 확인
        CheckBattleEnd();
    }

    [ContextMenu("전투 종료")]
    public void BattleEnd()
    {
        Debug.Log("[Battle Manager] 전투 종료");
        OnBattleEnd?.Invoke();
        currentBattleState = BattleState.RoundEnd;

        if (delayRountine != null)
        {
            StopCoroutine(delayRountine);
            delayRountine = null;
        }

        delayRountine = StartCoroutine(
        DelayRountine(roundEndDuration, () =>
        {
            RewardManager.Instance.StartRewardPhase();

            RoundEnd();
        }));
    }

    /// <summary>
    /// 배틀 종료 여부 확인, 종료 가능하다면 종료
    /// </summary>
    public void CheckBattleEnd()
    {
        if (currentBattleState != BattleState.Combat) return;

        if (UnitManager.Instance.IsBattleEnd())
        {
            BattleEnd();
        }
    }

    [ContextMenu("라운드 종료")]
    public void RoundEnd()
    {
        UnitManager.Instance.ClaerEnemy();

        Debug.Log("[Battle Manager] 라운드 종료");
        OnRoundEnd?.Invoke();

    }

    public void NextRound()
    {
        // 다음 라운드로 넘어가기
        StageManager.Instance.SetNextRound();

        // TODO : StageManager 쪽에서 진행 하는것으로 수정
        // 라운드 넘기기
        //if (delayRountine != null)
        //{
        //    StopCoroutine(delayRountine);
        //    delayRountine = null;
        //}

        //delayRountine = StartCoroutine(DelayRountine(nextRoundDuration, () => RoundStart()));
    }

    private void BindRewardUI()
    {
        var reward = RewardManager.Instance;

        reward.OnSelectReward -= HandleRewardSelect;
        reward.OnSkip -= HandleRewardSkip;

        reward.OnSelectReward += HandleRewardSelect;
        reward.OnSkip += HandleRewardSkip;
    }

    private void HandleRewardSelect(CardDataSO card)
    {
        if (card == null)
        {
            Debug.LogError("Reward에서 null 카드 들어옴");
            return;
        }

        if (string.IsNullOrEmpty(card.cardId))
        {
            Debug.LogError("cardId 비어있는 카드 들어옴");
            return;
        }

        DeckManager.Instance.TryAddCardToDeck(card);

        NextRound();
        // TODO: 카드 풀 시스템 생기면 추가
        // CardPoolManager.Instance?.RemoveCard(card);
    }

    private void HandleRewardSkip()
    {
        NextRound();
    }

    private IEnumerator DelayRountine(float delay, Action Callback)
    {
        duration = delay;

        while(duration > 0)
        {
            duration -= Time.deltaTime;
            yield return null;
        }

        duration = 0;

        Callback?.Invoke();
    }

    void OnGUI()
    {
        // 텍스트 표시
        if(StageManager.Instance != null)
        GUI.Label(new Rect(10, 10, 300, 40), $"{StageManager.Instance.CurStageIndex + 1} - {StageManager.Instance.CurRoundIndex + 1} 스테이지 : {StageManager.Instance.CurrentRound?.roundType} 라운드");
        GUI.Label(new Rect(10, 30, 300, 40), $"현재 상태 : {currentBattleState}");

        if (currentBattleState == BattleState.Prepare)
        {
            // 버튼
            if (GUI.Button(new Rect(10, 60, 150, 40), "전투 시작"))
            {
                BattleStart();
            }
        }
        else
        {
            GUI.Label(new Rect(10, 50, 310, 40), $"남은 시간 : {duration:F0}초");
        }
    }
}
