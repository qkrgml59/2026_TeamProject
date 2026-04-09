#if UNITY_EDITOR
using GameEditor.Utility;
#endif
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unit;
using Item;
using UnityEngine.UI;
using UnityEditor;

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
        private RoundType roundType = RoundType.Normal;
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


        [Header("저장용 컴포넌트")]
        public TMP_InputField nameField;
        public Button saveButton;
        public Button newDataButton;
        public Button copyButton;
        public Button deleteButton;

        private void Start()
        {
            if(stageDropdown != null) stageDropdown.onValueChanged.AddListener(OnStageChanged);
            if (roundTypeDropdown != null) roundTypeDropdown.onValueChanged.AddListener(OnTypeChanged);
            if (roundDropdown != null) roundDropdown.onValueChanged.AddListener(OnRoundChanged);

            // 버튼 설정
            if (saveButton != null) saveButton.onClick.AddListener(Save);
            if (newDataButton != null) newDataButton.onClick.AddListener(NewData);
            if (copyButton != null) copyButton.onClick.AddListener(Copy);
            if (deleteButton != null) deleteButton.onClick.AddListener(Delete);

            StageRefresh();
            InitUnitButton();
            // TODO : 아이템 정보 추가
        }

        private void OnDestroy()
        {
            if (stageDropdown != null) stageDropdown.onValueChanged.RemoveListener(OnStageChanged);
            if (roundTypeDropdown != null) roundTypeDropdown.onValueChanged.RemoveListener(OnTypeChanged);
            if (roundDropdown != null) roundDropdown.onValueChanged.RemoveListener(OnRoundChanged);

            // 버튼 설정
            if (saveButton != null) saveButton.onClick.RemoveListener(Save);
            if (newDataButton != null) newDataButton.onClick.RemoveListener(NewData);
            if (copyButton != null) copyButton.onClick.RemoveListener(Copy);
            if (deleteButton != null) deleteButton.onClick.RemoveListener(Delete);
        }

        public void Save()
        {
            if (curRound == null) return;

            var unitList = GridManager.Instance.GetUnitSaveData();
            curRound.units = unitList;

#if UNITY_EDITOR
            if (nameField != null & nameField.text != curRound.name)
                Rename(curRound, nameField.text);

            EditorUtility.SetDirty(curRound);
#endif

            int index = roundList.IndexOf(curRound);

            SetRoundDropdown();
            roundDropdown.value = index + 1;
        }

        void Rename(RoundData data, string newName)
        {
            string path = AssetDatabase.GetAssetPath(data);

            AssetDatabase.RenameAsset(path, newName);
            AssetDatabase.SaveAssets();
        }
        public void Delete()
        {
            if (curStage == null || curRound == null) return;

#if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(curRound);

            roundList.Remove(curRound);

            AssetDatabase.DeleteAsset(path);

            EditorUtility.SetDirty(curStage);
            AssetDatabase.SaveAssets();
#endif

            RoundDropdownClear();
            SetRoundDropdown();
        }

        public void NewData()
        {
            if (curStage == null) return;
#if UNITY_EDITOR
            RoundData newData = CreateStageSO(curStage.themeType);
#endif
            if (newData == null) return;
            if (nameField == null) return;

            roundList.Add(newData);

            SetRoundDropdown();
            roundDropdown.value = roundList.Count;          // 마지막으로 추가된 요소
        }

        public void Copy()
        {
            if (curStage == null || curRound == null) return;
#if UNITY_EDITOR
            RoundData newData = CopyStageSO(curRound);
#endif
            if (newData == null) return;
            if (nameField == null) return;

            roundList.Add(newData);

            SetRoundDropdown();
            OnRoundChanged(roundList.Count);
        }

        RoundData CreateStageSO(ThemeType theme)
        {
            string folderPath = GetThemeFolderPath(theme);

            RoundData newSO = ScriptableObject.CreateInstance<RoundData>();

            // 기본 데이터 설정
            newSO.roundType = roundType;
            newSO.rewardCount = roundType == RoundType.Normal ? 1 : 2;

            string dataName = GetNewDataName();

            string path = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{dataName}.asset");

            AssetDatabase.CreateAsset(newSO, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return newSO;
        }

        string GetNewDataName()
        {
            string dataName;
            if (curStage != null)
                dataName = $"{curStage.themeType}_";
            else
                dataName = "Stage_";

            dataName += $"{roundType}_";

            dataName += (roundList.Count + 1).ToString("D3");

            return dataName;
        }

        RoundData CopyStageSO(RoundData original)
        {
            string folderPath = GetThemeFolderPath(curStage.themeType);

            string originalPath = AssetDatabase.GetAssetPath(original);

            string newPath = AssetDatabase.GenerateUniqueAssetPath(
                $"{folderPath}/{original.name}_clone.asset"
            );

            AssetDatabase.CopyAsset(originalPath, newPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return AssetDatabase.LoadAssetAtPath<RoundData>(newPath);
        }

        string GetThemeFolderPath(ThemeType theme)
        {
            string themePath = $"{stageDataPath}/{theme}";

            if (!AssetDatabase.IsValidFolder(themePath))
            {
                AssetDatabase.CreateFolder(stageDataPath, theme.ToString());
            }

            return themePath;
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

                button.gameObject.SetActive(false);
            }
        }

        void SetActiveAllUnitButton(bool isActive)
        {
            foreach (var button in unitPlacerButtons)
            {
                button.gameObject.SetActive(isActive);
            }
        }

        /// <summary>
        /// 동일 테마의 유닛 버튼만 활성화
        /// </summary>
        void ActiveSameThemeUnit()
        {
            ThemeType theme = curStage.themeType;

            foreach(var button in unitPlacerButtons)
            {
                button.gameObject.SetActive(button.data.Race == theme || button.data.Race == ThemeType.Default);
            }
        }

        #region Stage Select

        void StageDropdownClear()
        {
            curStage = null;

            stageDropdown.ClearOptions();
            stageDropdown.interactable = false;

            OnStageChanged(0);
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

            index -= 1;
            if(index < 0 || index >= stageDatas.Count)
            {
                curStage = null;
                stageDropdown.SetValueWithoutNotify(-1);
                stageDropdown.captionText.text = "스테이지";
                SetActiveAllUnitButton(false);
                return;
            }

            curStage = stageDatas[index];

            if(curStage != null)
            {
                SetTypeDropdown();
                ActiveSameThemeUnit();
            }
        }
        #endregion

        #region RoundType Select

        void RoundTypeDropdownClear()
        {
            roundList = null;

            roundTypeDropdown.ClearOptions();
            roundTypeDropdown.interactable = false;

            OnTypeChanged(0);
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

            index -= 1;
            if (index < 0 || index > 2)
            {
                roundList.Clear();
                roundTypeDropdown.SetValueWithoutNotify(-1);
                roundTypeDropdown.captionText.text = "라운드 종류";

                if (newDataButton != null)
                    newDataButton.interactable = false;

                return;
            }

            switch(index)
            {
                case 0:
                    roundList = curStage.normalRound;
                    roundType = RoundType.Normal;
                    break;
                case 1:
                    roundList = curStage.eliteRound;
                    roundType = RoundType.Elite;
                    break;
                case 2:
                    roundList = curStage.bossRound;
                    roundType = RoundType.Boss;
                    break;
            }

            if (roundList.Count > 0)
            {
                SetRoundDropdown();
            }

            if (newDataButton != null)
                newDataButton.interactable = true;
        }
        #endregion

        #region Round Select

        void RoundDropdownClear()
        {
            curRound = null;
            GridManager.Instance.GridReset();

            roundDropdown.ClearOptions();
            roundDropdown.interactable = false;

            OnRoundChanged(0);
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

        void OnRoundChanged(int index)
        {
            index -= 1;
            if (index < 0 || index >= roundList.Count)
            {
                curRound = null;
                roundDropdown.SetValueWithoutNotify(-1);
                roundDropdown.captionText.text = "라운드";

                GridManager.Instance.GridReset();

                if (saveButton != null) saveButton.interactable = false;
                if (deleteButton != null) deleteButton.interactable = false;
                if (copyButton != null) copyButton.interactable = false;

                if (nameField != null)
                    nameField.text = "";

                return;
            }

            curRound = roundList[index];

            if (curRound != null)
            {
                GridManager.Instance.SetUnits(curRound.units);

                if (nameField != null)
                    nameField.text = curRound.name;
            }

            if (saveButton != null) saveButton.interactable = curRound != null;
            if (deleteButton != null) deleteButton.interactable = curRound != null;
            if (copyButton != null) copyButton.interactable = curRound != null;

            
        }
        #endregion

    }
}
