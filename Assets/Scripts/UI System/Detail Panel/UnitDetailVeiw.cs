using Game.UI;
using StatSystem;
using System.Collections.Generic;
using TMPro;
using Unit;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{

    public class UnitDetailVeiw : MonoBehaviour
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


        private UnitBase _unit;

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

        public void Bind(UnitBase unit)
        {
            if(unit == null) return;
            _unit = unit;

            if (_unit.UnitData != null)
            {
                if (nameText != null) nameText.text = _unit.UnitData.Name_KR;
                if (profile != null) profile.sprite = _unit.UnitData.unitSprite ?? dummyImage;
            }

            if (hpView != null)
            {
                _unit.unitEvents.OnHpChanged.AddListener(hpView.OnHpChanged);
                hpView.SetTeamColor(unit.team);
            }

            if (resourceView != null) _unit.unitEvents.OnResourceChanged.AddListener(resourceView.OnResourceChanged);

            if (itemView != null) _unit.unitEvents.OnItemChanged.AddListener(itemView.UpdateItems);

            // UI 강제 초기화
            _unit.RefreshAllUnitInfo();


            if (unit.statSet == null) return;
            StatSet set = unit.statSet;
            
            foreach(var view in views)
            {
                Stat stat = set.Get(view.Key);
                if (stat == null) continue;

                // view에 해당하는 스텟을 받아 이벤트 등록
                stat.onValueChanged += OnStatChanged;

                // 기본값 초기화
                OnStatChanged(stat);
            }
        }

        // 스텟 변경 이벤트를 받아 텍스트를 띄워줌
        void OnStatChanged(Stat stat)
        {
            if(views.TryGetValue(stat.statType, out UnitStatView view))
            {
                string text = StatFormatter.StatToString(stat.Value, stat.statType);
                view.UpdateView(text);
            }
        }

        public void UnBind()
        {
            if (_unit == null) return;

            if (nameText != null) nameText.text = "";

            if (hpView != null)
            {
                _unit.unitEvents.OnHpChanged.RemoveListener(hpView.OnHpChanged);
                hpView.Clear();             // 초기화
            }

            if (resourceView != null)
            {
                _unit.unitEvents.OnResourceChanged.RemoveListener(resourceView.OnResourceChanged);
                resourceView.Clear();       // 초기화
            }

            if (itemView != null)
            {
                _unit.unitEvents.OnItemChanged.RemoveListener(itemView.UpdateItems);
                itemView.Clear();
            }

            // 스텟 관련 이벤트 제거
            if (_unit.statSet == null) return;
            StatSet set = _unit.statSet;

            foreach (var view in views)
            {
                view.Value.Clear();

                Stat stat = set.Get(view.Key);
                if (stat == null) continue;

                // view에 해당하는 스텟을 받아 이벤트 제거
                stat.onValueChanged -= OnStatChanged;
            }
        }
    } 
}
