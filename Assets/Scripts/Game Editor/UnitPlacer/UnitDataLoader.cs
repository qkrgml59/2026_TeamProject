#if UNITY_EDITOR
using GameEditor.Utility;
#endif
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unit;
using Item;

namespace GameEditor.UnitPlacer
{
    public class UnitDataLoader : MonoBehaviour
    {
        public Canvas canvas;

        [Header("스테이지 데이터 경로")]
        public string stageDataPath = "Assets/GameResources/ScriptableObjects/StageData";

        // 현재 스테이지/라운드 정보
        private List<StageData> stageDatas = new();
        private StageData curStage;
        private List<RoundData> roundList = new();
        private RoundData curRound;

        [Header("스테이지 드롭다운")]
        public TMP_Dropdown stageDropdown;
        public TMP_Dropdown roundTypeDropdown;
        public TMP_Dropdown roundDropdown;

        [Header("유닛 데이터 경로")]
        public string unitDataPath = "Assets/GameResources/ScriptableObjects/UnitData";

        [Header("유닛 버튼 UI 컴포넌트")]
        public UnitPlacerButton unitButtonPrefab;
        public RectTransform scrollVeiw;

        public List<UnitDataSO> unitDatas = new();
        private List<UnitPlacerButton> unitPlacerButtons = new();

        private List<ItemSO> itemSO = new();

        private void Start()
        {
            stageDropdown.onValueChanged.AddListener(OnStageChanged);
            roundTypeDropdown.onValueChanged.AddListener(OnTypeChanged);
            roundDropdown.onValueChanged.AddListener(OnRoundhanged);

            StageRefresh();
            InitUnitButton();
            // TODO : 아이템 정보 추가
        }

        private void OnDestroy()
        {
            stageDropdown.onValueChanged.RemoveListener(OnStageChanged);
            roundTypeDropdown.onValueChanged.RemoveListener(OnTypeChanged);
            roundDropdown.onValueChanged.RemoveListener(OnRoundhanged);
        }

        public void StageRefresh()
        {
            #if UNITY_EDITOR
            stageDatas = AssetLoader.LoadAll<StageData>(stageDataPath);
            #endif

            StageDropdownClear();
            RoundTypeDropdownClear();
            RoundDropdownClear();

            if (stageDatas.Count == 0)
                return;

            SetStageDropdown();
        }

        void InitUnitButton()
        {
            #if UNITY_EDITOR
            unitDatas = AssetLoader.LoadAll<UnitDataSO>(unitDataPath);
            #endif

            for (int i = 0; i < unitDatas.Count; i++)
            {
                UnitPlacerButton button = Instantiate(unitButtonPrefab, scrollVeiw);
                button.Init(canvas, unitDatas[i]);
                unitPlacerButtons.Add(button);
            }
        }

        #region Stage Select

        void StageDropdownClear()
        {
            curStage = null;

            stageDropdown.ClearOptions();
            stageDropdown.interactable = false;

            stageDropdown.SetValueWithoutNotify(-1);
            stageDropdown.captionText.text = "스테이지";
        }

        void SetStageDropdown()
        {
            stageDropdown.ClearOptions();

            stageDropdown.options.Add(new TMP_Dropdown.OptionData("스테이지"));

            for (int i = 0; i < stageDatas.Count; i++)
            {
                stageDropdown.options.Add(new TMP_Dropdown.OptionData(stageDatas[i].themeType.ToString()));
            }

            stageDropdown.RefreshShownValue();

            if (stageDropdown.options.Count > 0)
                stageDropdown.interactable = true;
        }

        void OnStageChanged(int index)
        {
            RoundTypeDropdownClear();
            RoundDropdownClear();

            if(index - 1 < 0 || index - 1 > stageDatas.Count)
            {
                stageDropdown.SetValueWithoutNotify(-1);
                stageDropdown.captionText.text = "스테이지";
                return;
            }

            curStage = stageDatas[index - 1];

            if(curStage != null)
            {
                SetTypeDropdown();
            }
        }
        #endregion

        #region RoundType Select

        void RoundTypeDropdownClear()
        {
            roundList = null;

            roundTypeDropdown.ClearOptions();
            roundTypeDropdown.interactable = false;

            roundTypeDropdown.SetValueWithoutNotify(-1);
            roundTypeDropdown.captionText.text = "라운드 종류";
        }

        void SetTypeDropdown()
        {
            roundTypeDropdown.ClearOptions();

            List<string> options = new List<string>()
            {
                "라운드 종류",
                "일반",
                "중간 보스",
                "보스"
            };

            roundTypeDropdown.AddOptions(options);

            roundTypeDropdown.RefreshShownValue();

            roundTypeDropdown.interactable = true;
        }

        void OnTypeChanged(int index)
        {
            RoundDropdownClear();

            if (curStage == null) return;

            if (index - 1 < 0 || index > 3)
            {
                roundTypeDropdown.SetValueWithoutNotify(-1);
                roundTypeDropdown.captionText.text = "라운드 종류";
                return;
            }

            switch(index - 1)
            {
                case 0:
                    roundList = curStage.normalRound;
                    break;
                case 1:
                    roundList = curStage.eliteRound;
                    break;
                case 2:
                    roundList = curStage.bossRound;
                    break;
            }

            if (roundList.Count > 0)
            {
                SetRoundDropdown();
            }
        }
        #endregion

        #region Round Select

        void RoundDropdownClear()
        {
            curRound = null;
            GridManager.Instance.GridReset();

            roundDropdown.ClearOptions();
            roundDropdown.interactable = false;

            roundDropdown.SetValueWithoutNotify(-1);
            roundDropdown.captionText.text = "라운드";
        }

        void SetRoundDropdown()
        {
            roundDropdown.ClearOptions();

            roundDropdown.options.Add(new TMP_Dropdown.OptionData("라운드"));

            for (int i = 0; i < roundList.Count; i++)
            {
                roundDropdown.options.Add(new TMP_Dropdown.OptionData(roundList[i].name));
            }

            roundDropdown.RefreshShownValue();

            if (roundDropdown.options.Count > 0)
                roundDropdown.interactable = true;
        }

        void OnRoundhanged(int index)
        {
            if (index - 1 < 0 || index - 1 > roundList.Count)
            {
                roundDropdown.SetValueWithoutNotify(-1);
                roundDropdown.captionText.text = "라운드";
                return;
            }

            curRound = roundList[index - 1];

            if(curRound != null)
                GridManager.Instance.SetUnits(curRound.units);
        }
        #endregion

    }
}
