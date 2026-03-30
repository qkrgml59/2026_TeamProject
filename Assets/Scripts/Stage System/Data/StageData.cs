using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "StageData/new Stage")]
public class StageData : ScriptableObject
{
    [Header("테마 종류")] public ThemeType themeType;
    [Header("일반 라운드 정보")] public List<RoundData> normalRound = new();
    [Header("정예 라운드 정보")] public List<RoundData> eliteRound = new();
    [Header("보스 라운드 정보")] public List<RoundData> bossRound = new();
    [Header("정비 라운드 정보")] public List<RoundData> restRound = new();
    [Header("이벤트 라운드 정보")] public List<RoundData> eventRound = new();

    /// <summary>
    /// 해당 테마에서 라운드 타입에 맞는 RoundData 반환
    /// </summary>
    public RoundData GetRandomRound(RoundType type)
    {
        int rand = 0;
        switch (type)
        {
            case RoundType.Normal:
                if (normalRound.Count == 0) return null;
                rand = Random.Range(0, normalRound.Count);
                return normalRound[rand];
            case RoundType.Elite:
                if (eliteRound.Count == 0) return null;
                rand = Random.Range(0, eliteRound.Count);
                return eliteRound[rand];
            case RoundType.Boss:
                if (bossRound.Count == 0) return null;
                rand = Random.Range(0, bossRound.Count);
                return bossRound[rand];
            case RoundType.Rest:
                if (restRound.Count == 0) return null;
                rand = Random.Range(0, restRound.Count);
                return restRound[rand];
            case RoundType.Event:
                if (eventRound.Count == 0) return null;
                rand = Random.Range(0, eventRound.Count);
                return eventRound[rand];
            default:
                return null;
        }
    }
}
