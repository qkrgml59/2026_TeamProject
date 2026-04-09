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
    public class PlacerDataManager : MonoBehaviour
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
        public UnitButton unitButtonPrefab;
        public RectTransform scrollVeiw;

        public List<UnitDataSO> unitDatas = new();
        private List<UnitButton> unitPlacerButtons = new();

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

            LoadStageData();
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

        /// <summary>
        /// 스테이지 데이터를 다시 불러옵니다.
        /// </summary>
        public void LoadStageData()
        {
#if UNITY_EDITOR
            stageDatas = AssetUtility.LoadAll<StageData>(stageDataPath);
#endif

            StageDropdownClear();
            RoundTypeDropdownClear();
            RoundDropdownClear();

            if (stageDatas.Count == 0)
                return;

            SetStageDropdown();
        }

        #region 데이터 관리

        // 저장
        public void Save()
        {
            if (curRound == null) return;

            // 유닛 배치 목록 불러오기
            var unitList = GridManager.Instance.GetUnitPlacementList();
            curRound.units = unitList;

            // 저장 및 이름 변경 진행
#if UNITY_EDITOR
            if (nameField != null & nameField.text != curRound.name)
                Rename(curRound, nameField.text);

            EditorUtility.SetDirty(curRound);
#endif

            // 드롭다운 리프래쉬 및 value 변경
            int index = roundList.IndexOf(curRound);
            SetRoundDropdown();
            roundDropdown.value = index + 1;
        }

        // 에셋 파일의 이름을 변경합니다.
        void Rename(RoundData data, string newName)
        {
            string path = AssetDatabase.GetAssetPath(data);

            AssetDatabase.RenameAsset(path, newName);
            AssetDatabase.SaveAssets();
        }

        // 현재 데이터 삭제
        public void Delete()
        {
            if (curStage == null || curRound == null) return;

#if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(curRound);
            // 리스트에서 먼저 삭제
            roundList.Remove(curRound);

            // 에셋 삭제
            AssetDatabase.DeleteAsset(path);
            EditorUtility.SetDirty(curStage);       // 리스트에 반영
            AssetDatabase.SaveAssets();
#endif

            RoundDropdownClear();
            SetRoundDropdown();
        }

        // 새로운 데이터 생성
        public void NewData()
        {
            if (curStage == null) return;
#if UNITY_EDITOR
            RoundData newData = CreateStageSO(curStage.themeType);
#endif
            if (newData == null) return;
            if (nameField == null) return;

            // 현재 라운드 리스트 마지막에 추가 후 드롭다운 설정
            roundList.Add(newData);
            SetRoundDropdown();
            roundDropdown.value = roundList.Count;
        }

        // 새로운 라운드 Asset 생성
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

        // 현재 데이터 복제
        public void Copy()
        {
            Save();

            if (curStage == null || curRound == null) return;
#if UNITY_EDITOR
            // 데이터 복제
            RoundData newData = CopyStageSO(curRound);
#endif
            if (newData == null) return;
            if (nameField == null) return;

            // 현재 라운드 리스트 마지막에 추가 후 드롭다운 설정
            roundList.Add(newData);
            SetRoundDropdown();
            roundDropdown.value = roundList.Count;
        }

        // 라운드 데이터 복사
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
        #endregion

        #region 유닛 버튼 관리

        // 모든 유닛 버튼 생성
        void InitUnitButton()
        {
            #if UNITY_EDITOR
            unitDatas = AssetUtility.LoadAll<UnitDataSO>(unitDataPath);
            #endif

            for (int i = 0; i < unitDatas.Count; i++)
            {
                UnitButton button = Instantiate(unitButtonPrefab, scrollVeiw);
                button.Init(canvas, unitDatas[i]);
                unitPlacerButtons.Add(button);

                button.gameObject.SetActive(false);
            }
        }

        // 전체 유닛 버튼 관리
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

        #endregion

        #region 스테이지

        // 스테이지 드롭다운 초기화
        void StageDropdownClear()
        {
            curStage = null;

            stageDropdown.ClearOptions();
            stageDropdown.interactable = false;

            OnStageChanged(0);
        }

        // 스테이지 드롭다운 설정
        void SetStageDropdown()
        {
            stageDropdown.ClearOptions();

            // 첫번째 항목은 고정
            stageDropdown.options.Add(new TMP_Dropdown.OptionData("스테이지"));

            for (int i = 0; i < stageDatas.Count; i++)
            {
                stageDropdown.options.Add(new TMP_Dropdown.OptionData(stageDatas[i].themeType.ToString()));
            }

            stageDropdown.RefreshShownValue();

            if (stageDropdown.options.Count > 0)
                stageDropdown.interactable = true;
        }

        /// <summary>
        /// 스테이지 드롭다운 값 변경
        /// </summary>
        void OnStageChanged(int index)
        {
            // 스테이지 변경 시 하위 드롭다운 초기화
            RoundTypeDropdownClear();
            RoundDropdownClear();

            index -= 1;
            if(index < 0 || index >= stageDatas.Count)
            {
                curStage = null;
                stageDropdown.SetValueWithoutNotify(-1);
                stageDropdown.captionText.text = "스테이지";

                // 모든 유닛 버튼 비활성화
                SetActiveAllUnitButton(false);
                return;
            }

            // 스테이지 설정
            curStage = stageDatas[index];

            if(curStage != null)
            {
                // 현재 스테이지가 정해진 경우
                SetTypeDropdown();          // 타입 드롭다운 설정
                ActiveSameThemeUnit();      // 사용 가능 유닛 목록 활성화
            }
        }
        #endregion

        #region 라운드 종류

        // 타입 드롭다운 초기화
        void RoundTypeDropdownClear()
        {
            roundList = new();

            roundTypeDropdown.ClearOptions();
            roundTypeDropdown.interactable = false;

            OnTypeChanged(0);
        }

        // 타입 드롭다운 설정
        void SetTypeDropdown()
        {
            roundTypeDropdown.ClearOptions();

            // 라운드 종류는 3가지로 고정
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

        /// <summary>
        /// 타입 드롭다운 값 변경
        /// </summary>
        void OnTypeChanged(int index)
        {
            // 라운드 종류가 변하는 경우 하위 드롭다운 초기화
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
                // 라운드 리스트가 있는 경우, 라운드 드롭다운 설정
                SetRoundDropdown();
            }

            if (newDataButton != null)
                newDataButton.interactable = true;
        }
        #endregion

        #region 라운드

        // 라운드 드롭다운을 비우고, 대기 상태로 변경
        void RoundDropdownClear()
        {
            curRound = null;
            GridManager.Instance.GridReset();

            roundDropdown.ClearOptions();
            roundDropdown.interactable = false;

            OnRoundChanged(0);
        }

        // 라운드 리스트를 참조하여 드롭다운 설정
        void SetRoundDropdown()
        {
            roundDropdown.ClearOptions();

            // 첫번째 옵션 고정
            roundDropdown.options.Add(new TMP_Dropdown.OptionData("라운드"));

            for (int i = 0; i < roundList.Count; i++)
            {
                roundDropdown.options.Add(new TMP_Dropdown.OptionData(roundList[i].name));
            }

            roundDropdown.RefreshShownValue();

            if (roundDropdown.options.Count > 0)
                roundDropdown.interactable = true;
        }

        /// <summary>
        /// 라운드 드롭다운 값 변경
        /// </summary>
        void OnRoundChanged(int index)
        {
            index -= 1;
            if (index < 0 || index >= roundList.Count)
            {
                // 맞지 않는 인덱스 또는 초기화의 경우
                curRound = null;
                roundDropdown.SetValueWithoutNotify(-1);
                roundDropdown.captionText.text = "라운드";

                // 그리드 비우기
                GridManager.Instance.GridReset();

                if (saveButton != null) saveButton.interactable = false;
                if (deleteButton != null) deleteButton.interactable = false;
                if (copyButton != null) copyButton.interactable = false;

                if (nameField != null)
                    nameField.text = "";

                return;
            }

            // 현재 라운드 설정
            curRound = roundList[index];

            if (curRound != null)
            {
                // 유닛 배치
                GridManager.Instance.SetUnits(curRound.units);

                if (nameField != null)
                    nameField.text = curRound.name;
            }

            if (saveButton != null) saveButton.interactable = curRound != null;
            if (deleteButton != null) deleteButton.interactable = curRound != null;
            if (copyButton != null) copyButton.interactable = curRound != null;
        }
        #endregion

        // 테마별 폴더를 불러옵니다.
        string GetThemeFolderPath(ThemeType theme)
        {
            string themePath = $"{stageDataPath}/{theme}";

            if (!AssetDatabase.IsValidFolder(themePath))
            {
                AssetDatabase.CreateFolder(stageDataPath, theme.ToString());
            }

            return themePath;
        }

        // 새롭게 생성하는 라운드 데이터의 이름을 반환합니다.
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
    }
}
