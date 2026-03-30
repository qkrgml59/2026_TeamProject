using System.Collections.Generic;
using UnityEngine;
using Utilitys;

public class StageManager : SingletonMonoBehaviour<StageManager>
{
    [Header("모든 스테이지 원본")]
    [SerializeField] private List<StageData> stageDatas = new List<StageData>();

    [Header("게임 설정")]
    public int stageCount = 3;
    public List<RoundType> stageCycle = new List<RoundType>() {
        RoundType.Normal,
        RoundType.Normal,
        RoundType.Elite,
        RoundType.Rest,
        RoundType.Normal,
        RoundType.Normal,
        RoundType.Elite,
        RoundType.Rest,
        RoundType.Normal,
        RoundType.Normal,
        RoundType.Boss,
        RoundType.Rest
    };

    [Header("라운드 정보(읽기 전용, 직접 수정 X)")]
    [SerializeField] private int currentRoundIndex = 0;
    [SerializeField] private RoundData currentRound;
    public RoundData CurrentRound => currentRound;

    [Header("스테이지 정보 (읽기 전용, 직접 수정 X)")]
    [SerializeField] private int currentStageIndex = 0;
    [SerializeField] private List<RoundData> currentStage = new();
    public IReadOnlyList<RoundData> CurrentStage => currentStage;

    void SelectStageThemes()
    {

    }

    void SetupStage()
    {

    }
}
