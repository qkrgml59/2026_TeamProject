using System.Collections.Generic;
using Unit;
using UnityEngine;


[CreateAssetMenu(fileName = "new RoundData", menuName = "StageData/new Round")]
public class RoundData : ScriptableObject
{
    [Header("라운드 종류")] public RoundType roundType;
    [Header("유닛 배치")] public List<StageUnitData> units = new();
    [Header("보상 정보")] public int rewardCount = 1;
}
