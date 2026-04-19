using Prototype.Card;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilitys;

public class StageManager : SingletonMonoBehaviour<StageManager>
{
    [Header("모든 스테이지 원본")]
    [SerializeField] private List<StageData> stageDatas = new List<StageData>();
    public Dictionary<ThemeType, StageData> stages { get; private set; } = new();

    [Header("스테이지 종료 시 코스트 회복량")]
    public int recoverCostAmout = 1;

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
    [SerializeField] private int curRoundIndex = -1;
    public int CurRoundIndex => curRoundIndex;
    [SerializeField] private RoundData currentRound;
    public RoundData CurrentRound => currentRound;


     private List<CardDataSO> unitCards = new();
     private List<CardDataSO> itemCards = new();
     private List<CardDataSO> spellCards = new();

    public List<CardDataSO> UnitPool => unitCards;
    public List<CardDataSO> SpellPool => spellCards;
    public List<CardDataSO> ItemPool => itemCards;


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
        //SetGameThemes(totalStageCount);

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

    
    ///<summary>
    ///타이틀 씬에서 선택한 테마 적용
    /// </summary>
    public void ApplySelectedThemes(List<ThemeType> selectedThemes)
    {
        gameThemes.Clear();
        gameThemes.AddRange(selectedThemes);

        // TODO : 스테이지 및 라운드 인덱스 초기화 (또는 스테이지/라운드 정보 초기화 기능 추가 필요)

        InitCardPool();

        currentRound = stages[CurStageTheme].GetRandomRound(stageCycle[CurRoundIndex]);

        Debug.Log("선택한 테마 적용 완료");
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

        if (CostManager.Instance != null)
            CostManager.Instance.RecoverCost(recoverCostAmout);

        Debug.Log($"[StageManager] 현재 : ({CurStageIndex + 1}스테이지 {CurRoundIndex + 1}라운드)");
        Debug.Log($"[StageManager] 스테이지 종료! ({recoverCostAmout + 1}코스트 회복");
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

    /// <summary>
    /// 카드팩 UI용 StageData 목록 반환
    /// (읽기 전용)
    /// </summary>
    public List<StageData> GetStageDataList()
    {
        return stageDatas;
    }

    //나중에 수정해야할듯
    // TODO : pack을 매개변수로 넘기지 말고, 테마 값으로 stage 딕셔너리에서 읽어오기
    public List<CardDataSO> GetPreviewCards(StageData pack, int count = 6)
    {
        List<CardDataSO> result = new List<CardDataSO>();

        List<CardDataSO> temp = new List<CardDataSO>();

        foreach (var e in pack.UnitCards)
            for (int i = 0; i < e.count; i++)
                temp.Add(e.cardData);

        foreach (var e in pack.ItemCards)
            for (int i = 0; i < e.count; i++)
                temp.Add(e.cardData);

        foreach (var e in pack.SpellCards)
            for (int i = 0; i < e.count; i++)
                temp.Add(e.cardData);

        for (int i = 0; i < count; i++)
        {
            if (temp.Count == 0) break;

            int rand = Random.Range(0, temp.Count);
            result.Add(temp[rand]);
        }

        return result;
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
        if (pool == null || pool.Count == 0)
        {
            Debug.LogError("카드 풀이 비어있음!");
            return null;
        }

        int rand = UnityEngine.Random.Range(0, pool.Count);
        CardDataSO cardData = pool[rand];
        pool.RemoveAt(rand);
        return cardData == null ? null : cardData;
    }

    //카드풀로 다시 반환
    public void ReturnCardToPool(CardDataSO card)
    {
        if (card == null) return;

        switch (card.CardType)
        {
            case CardType.Unit:
                unitCards.Add(card);
                break;
            case CardType.Item:
                itemCards.Add(card);
                break;
            case CardType.Spell:
                spellCards.Add(card);
                break;
            default:
                Debug.LogWarning("알 수 없는 카드 타입");
                break;
        }
    }

    /// <summary>
    /// 미리보기용 카드 반환
    /// </summary>
    public List<CardDataSO> PeekPackPreviewCards(int count)
    {
        List<CardDataSO> result = new List<CardDataSO>();

        for (int i = 0; i < count; i++)
        {
            if (unitCards.Count == 0)
                break;

            int rand = Random.Range(0, unitCards.Count);
            result.Add(unitCards[rand]);
        }

        return result;
    }

    //팩 미리보기 용 카드
    public List<CardDataSO> GetPackPreviewCards(int count)
    {
        List<CardDataSO> result = new List<CardDataSO>();

        for (int i= 0; i< count; i++)
        {
            var card = GetRandomCardData(CardType.Unit);

            if (card != null)
                result.Add(card);
        }

        return result;
    }

    #endregion
}
