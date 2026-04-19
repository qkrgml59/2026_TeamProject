using Game.UI;
using Prototype.Card.Unit;
using StatSystem;
using System.Collections.Generic;
using TMPro;
using Unit;
using UnityEngine;
using UnityEngine.UI;

public class UnitCardDetailView : MonoBehaviour
{
    [Header("디폴트 이미지")] public Sprite dummyImage;

    [Header("기본 정보")]
    public TextMeshProUGUI nameText;
    public Image profile;

    [Header("Bar View")]
    public UnitHpView hpView;
    public UnitResourceView resourceView;

    [Header("Item View")]
    public UnitItemView itemView;

    [Header("Stat View")]
    public UnitStatView ADView;
    public UnitStatView APView;
    public UnitStatView OmnivampView;
    public UnitStatView DefenseView;
    public UnitStatView MagicResistanceView;
    public UnitStatView AttackSpeedView;
    public UnitStatView CritChanceView;
    public UnitStatView CritDamageView;
    public UnitStatView IncreaseView;
    public UnitStatView ReductionView;

    Dictionary<StatType, UnitStatView> views = new();

    UnitDataSO _unitData;
    StatSet _statSet;

    private void Awake()
    {
        // 수동으로 캐싱
        if (ADView != null) views[StatType.AttackDamage] = ADView;
        if (APView != null) views[StatType.AbilityPower] = APView;
        if (DefenseView != null) views[StatType.Defense] = DefenseView;
        if (MagicResistanceView != null) views[StatType.MagicResistance] = MagicResistanceView;
        if (OmnivampView != null) views[StatType.Omnivamp] = OmnivampView;
        if (AttackSpeedView != null) views[StatType.AttackSpeed] = AttackSpeedView;
        if (CritChanceView != null) views[StatType.CriticalChance] = CritChanceView;
        if (CritDamageView != null) views[StatType.CriticalDamage] = CritDamageView;
        if (IncreaseView != null) views[StatType.DamageIncrease] = IncreaseView;
        if (ReductionView != null) views[StatType.DamageReduction] = ReductionView;
    }

    public void Show(UnitCardDataSO unitCard)
    {
        if (unitCard == null || unitCard.unitDataSO == null) return;

        if(unitCard.unitDataSO != _unitData)
            _unitData = unitCard.unitDataSO;

        // TODO : 스텟 변경이 있었는지 확인 (성급 변화 등)
        if(unitCard.unitDataSO.statData != null)
        _statSet = new StatSet(_unitData.statData, 0);          // TODO : 유닛 성급 적용

        if (nameText != null) nameText.text = _unitData.Name_KR;
        if (profile != null) profile.sprite = unitCard.icon ?? dummyImage;

        if (hpView != null)
        {
            float hp = _statSet.MaxHp.Value;
            hpView.OnHpChanged(new HealthInfo(hp, hp, 0));
            hpView.SetTeamColor(TeamType.Ally);
        }

        if (resourceView != null && _unitData.Skill_Prefab != null)
            resourceView.OnResourceChanged(new ResourceInfo(ResourceType.Mana, 0, _unitData.Skill_Prefab.Cost));     // TODO : 코스트 타입이나, 시작 코스트 반영

        if (itemView != null) itemView.Clear();

        // 스텟 초기화

        foreach (var view in views)
        {
            Stat stat = _statSet.Get(view.Key);
            if (stat == null) continue;

            // 기본값 초기화
            string text = StatFormatter.StatToString(stat.Value, stat.statType);
            view.Value.UpdateView(text);
        }
    }


    public void Clear()
    {
        if (nameText != null) nameText.text = "";

        if (hpView != null) hpView.Clear();             // 초기화

        if (resourceView != null) resourceView.Clear();       // 초기화

        if (itemView != null) itemView.Clear();

        foreach (var view in views)
        {
            view.Value.Clear();
        }
    }
}
