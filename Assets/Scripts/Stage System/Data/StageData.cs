using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "StageData/new Stage")]
public class StageData : ScriptableObject
{
    [Header("테마 종류")] public ThemeType themeType;
    [Header("일반 라운드 정보")] public List<RoundData> normalRound = new();
    [Header("정예 라운드 정보")] public List<RoundData> eliteRound = new();
    [Header("보스 라운드 정보")] public List<RoundData> bossRound = new();
}
