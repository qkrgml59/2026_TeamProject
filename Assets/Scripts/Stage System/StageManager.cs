using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilitys;

public class StageManager : SingletonMonoBehaviour<StageManager>
{
    [Header("모든 스테이지 원본")]
    [SerializeField] private List<StageData> stageDatas = new List<StageData>();

    [Header("게임 설정")]
    public int totalStageCount = 3;
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

    [Header("전채 테마 정보")]
    [SerializeField] private List<ThemeType> stageThemes = new List<ThemeType>();

    [Header("스테이지 정보 (읽기 전용, 직접 수정 X)")]
    [SerializeField] private int currentStageIndex = 0;
    [SerializeField] private List<RoundData> currentStage = new();
    public IReadOnlyList<RoundData> CurrentStage => currentStage;

    [Header("라운드 정보(읽기 전용, 직접 수정 X)")]
    [SerializeField] private int currentRoundIndex = 0;
    [SerializeField] private RoundData currentRound;
    public RoundData CurrentRound => currentRound;

    private void Start()
    {
        SelectStageThemes();
    }


    /// <summary>
    /// 스테이지 테마를 새롭게 설정
    /// </summary>
    public void SelectStageThemes()
    {
        stageThemes.Clear();
        currentStageIndex = -1;

        var themeTypes = new List<StageData>(stageDatas.ToArray());

        if (totalStageCount > themeTypes.Count)
        {
            Debug.LogWarning($"총 스테이지 개수가 테마의 수보다 많습니다.");
            return;
        }

        for(int i = 0; i < totalStageCount; i++)
        {
            int rand = Random.Range(0, themeTypes.Count);
            stageThemes.Add(themeTypes[rand].themeType);
            themeTypes.RemoveAt(rand);
        }

        Debug.Log($"[StageManager] 총 {totalStageCount}개의 스테이지 설정");
        SetNextRound();
    }

    /// <summary>
    /// 새로운 스테이지 설정
    /// </summary>
    public void SetNextStage()
    {
        currentStage.Clear();
        currentRoundIndex = -1;

        currentStageIndex++;

        if (currentStageIndex >= stageThemes.Count)
        {
            // 마지막 스테이지 완료
            Debug.Log("게임 클리어!");
            return;
        }

        StageData stage = stageDatas.Find(s => s.themeType == stageThemes[currentStageIndex]);

        for (int i = 0; i < stageCycle.Count; i++)
        {
            var round = stage.GetRandomRound(stageCycle[i]);
            if (round != null)
                currentStage.Add(round);
        }

        SetNextRound();
    }

    /// <summary>
    /// 다음 라운드로 변경
    /// </summary>
    public void SetNextRound()
    {
        currentRoundIndex++;

        if (currentRoundIndex >= currentStage.Count)
        {
            // 마지막 라운드(정비) 완료
            SetNextStage();
            return;
        }

        currentRound = currentStage[currentRoundIndex];
        Debug.Log($"[StageManager] 현재 : ({currentStageIndex + 1}스테이지 {currentRoundIndex + 1}라운드)");
    }
}
