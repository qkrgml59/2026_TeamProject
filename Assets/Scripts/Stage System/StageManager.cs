using Prototype.Card;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilitys;

public class StageManager : SingletonMonoBehaviour<StageManager>
{
    [Header("모든 스테이지 원본")]
    [SerializeField] private List<StageData> stageDatas = new List<StageData>();
    public Dictionary<ThemeType, StageData> stages = new();

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

    [Header("이번 게임의 테마 정보")]
    [SerializeField] private List<ThemeType> gameThemes = new List<ThemeType>();
    public ThemeType CurStageTheme => CurStageIndex < gameThemes.Count ? gameThemes[CurStageIndex] : ThemeType.Horror;

    [Header("스테이지 정보 (읽기 전용, 직접 수정 X)")]
    [SerializeField] private int curStageIndex = 0;
    public int CurStageIndex => curStageIndex;

    [Header("라운드 정보(읽기 전용, 직접 수정 X)")]
    [SerializeField] private int curRoundIndex = 0;
    public int CurRoundIndex => curRoundIndex;
    [SerializeField] private RoundData currentRound;
    public RoundData CurrentRound => currentRound;


     private List<CardDataSO> unitCards = new();
     private List<CardDataSO> itemCards = new();
     private List<CardDataSO> spellCards = new();

    protected override void OnSingletonAwake()
    {
        // 스테이지 정보 캐싱
        stages.Clear();
        foreach(StageData stage in stageDatas)
        {
            stages[stage.themeType] = stage;
        }
    }

    private void Start()
    {
        SetGameThemes(totalStageCount);

        InitCardPool();

        // 첫 라운드 세팅
        currentRound = stages[CurStageTheme].GetRandomRound(stageCycle[CurRoundIndex]);
    }

    /// <summary>
    /// 스테이지 테마를 새롭게 설정
    /// </summary>
    public void SetGameThemes(int count)
    {
        gameThemes.Clear();

        if (count > stageDatas.Count)
        {
            Debug.LogWarning($"요청한 개수({count})가 전체 테마 수({stageDatas.Count})보다 많습니다.");
            count = stageDatas.Count;
        }

        for(int i = 0; i < count; i++)
        {
            int rand = Random.Range(i, stageDatas.Count);
            gameThemes.Add(stageDatas[rand].themeType);
            (stageDatas[i], stageDatas[rand]) = (stageDatas[rand], stageDatas[i]);
        }

        Debug.Log($"{count}개의 테마 설정 완료");
    }

    /// <summary>
    /// 다음 라운드로 변경
    /// </summary>
    public void SetNextRound()
    {
        curRoundIndex++;
        if (curRoundIndex >= stageCycle.Count)
        {
            // TODO : 새로운 스테이지 진입 구현 필요. 일단 반복 
            curRoundIndex = -1;
            SetNextRound();
            return;
        }

        if (!stages.ContainsKey(CurStageTheme))
        {
            Debug.LogWarning($"{CurStageTheme} 테마 정보가 없습니다.");
            return;
        }

        currentRound = stages[CurStageTheme].GetRandomRound(stageCycle[CurRoundIndex]);

        if (BattleManager.Instance != null)
            BattleManager.Instance.RoundStart();

        Debug.Log($"[StageManager] 현재 : ({CurStageIndex + 1}스테이지 {CurRoundIndex + 1}라운드)");
    }

    #region 카드 관련 기능

    void InitCardPool()
    {
        unitCards.Clear();
        itemCards.Clear();
        spellCards.Clear();

        foreach (ThemeType theme in gameThemes)
        {
            if (!stages.ContainsKey(theme))
            {
                Debug.LogWarning($"{theme} 테마 정보가 없습니다.");
                continue;
            }

            SetCardPool(unitCards, stages[theme].UnitCards);
            SetCardPool(itemCards, stages[theme].ItemCards);
            SetCardPool(spellCards, stages[theme].SpellCards);
        }

        Debug.Log($"카드 풀 세팅 완료 (유닛 : {unitCards.Count}종 | 아이템 : {itemCards.Count}종 | 마법 : {spellCards.Count}종)");
    }

    void SetCardPool(List<CardDataSO> pool, List<CardEntry> entries)
    {
        foreach (var entry in entries)
        {
            for(int i = 0; i < entry.count; i++)
            {
                pool.Add(entry.cardData);
            }
        }
    }

    public CardDataSO GetRandomCardData(CardType type)
    {
        switch(type)
        {
            case CardType.Unit:
               return GetRandomCardFormPool(unitCards);
            case CardType.Item:
                return GetRandomCardFormPool(itemCards);
            case CardType.Spell:
                return GetRandomCardFormPool(spellCards);
            default:
            return null;
        }
    }

    public CardDataSO GetRandomCardFormPool (List<CardDataSO> pool)
    {
        int rand = UnityEngine.Random.Range(0, pool.Count);
        CardDataSO cardData = pool[rand];
        pool.RemoveAt(rand);
        return cardData == null ? null : cardData;
    }
    #endregion
}
