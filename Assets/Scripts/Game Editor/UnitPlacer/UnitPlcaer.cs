#if UNITY_EDITOR
using GameEditor.Utility;
#endif
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace GameEditor.UnitPlacer
{
    public class UnitDataLoader : MonoBehaviour
    {
        [Header("Stage Data Path")]
        public string stageDataPath = "Assets/GameResources/ScriptableObjects/StageData";

        public List<StageData> stageDatas = new();

        public TMP_Dropdown stageDropdown;
        public TMP_Dropdown roundTypeDropdown;
        public TMP_Dropdown roundDropdown;

        public StageData curStage;
        public List<RoundData> roundList = new();
        public RoundData curRound;

        private void Start()
        {
            stageDropdown.onValueChanged.AddListener(OnStageChanged);
            roundTypeDropdown.onValueChanged.AddListener(OnTypeChanged);
            roundDropdown.onValueChanged.AddListener(OnRoundhanged);

            Refresh();
        }

        private void OnDestroy()
        {
            stageDropdown.onValueChanged.RemoveListener(OnStageChanged);
            roundTypeDropdown.onValueChanged.RemoveListener(OnTypeChanged);
            roundDropdown.onValueChanged.RemoveListener(OnRoundhanged);
        }

        public void Refresh()
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
