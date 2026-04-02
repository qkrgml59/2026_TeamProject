using Prototype.Card;
using Prototype.Card.Item;
using Prototype.Card.Spell;
using Prototype.Card.Unit;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "StageData/new Stage")]
public class StageData : ScriptableObject
{
    [Header("테마 종류")]
    public ThemeType themeType;

    [Header("카드 정보")]
    [SerializeField] private List<CardEntry> unitCards = new();
    public List<CardEntry> UnitCards => unitCards;
    [SerializeField] private List<CardEntry> itemCards = new();
    public List<CardEntry> ItemCards => itemCards;
    [SerializeField] private List<CardEntry> spellCards = new();
    public List<CardEntry> SpellCards => spellCards;


    [Header("라운드 정보")]
    [SerializeField] private List<RoundData> normalRound = new();
    [SerializeField] private List<RoundData> eliteRound = new();
    [SerializeField] private List<RoundData> bossRound = new();



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
            default:
                return null;
        }
    }
}
