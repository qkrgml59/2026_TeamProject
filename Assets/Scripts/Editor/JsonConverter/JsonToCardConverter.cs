#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Prototype.Card;
using Prototype.Card.Unit;
using Prototype.Card.Spell;
using Unit;
using Unit.Skill;
using Item;
using StatSystem;


namespace GameEditor.JsonConverter
{
    public class JsonToCardConverter : EditorWindow
    {
        // 체크박스용 bool 변수 추가
        private bool convertUnit = true;
        private bool convertCard = true;
        private bool convertItem = true;

        // JSON 파일 경로 저장 변수
        private string unitJsonPath = "";
        private string cardJsonPath = "";
        private string itemJsonPath = "";

        // 데이터 저장 경로
        private readonly string unitDataPath = "Assets/GameResources/ScriptableObjects/UnitData";
        private readonly string statDataPath = "Assets/GameResources/ScriptableObjects/StatData";
        private readonly string cardDataPath = "Assets/GameResources/ScriptableObjects/CardData";
        private readonly string itemDataPath = "Assets/GameResources/ScriptableObjects/ItemData";

        [MenuItem("Tools/1. JSON to Data Converter")]
        public static void ShowWindow()
        {
            GetWindow<JsonToCardConverter>("Data Converter Tool");
        }

        void OnGUI()
        {
            GUILayout.Label("Phase 1: Factory Converter", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // [유닛 UI 영역]
            GUILayout.BeginVertical("box");
            convertUnit = EditorGUILayout.ToggleLeft(" 1. Unit Data 변환하기", convertUnit, EditorStyles.boldLabel);
            if (convertUnit)
            {
                if (GUILayout.Button("Select Unit JSON"))
                    unitJsonPath = EditorUtility.OpenFilePanel("Select Unit JSON", "", "json");
                EditorGUILayout.LabelField("Path: ", string.IsNullOrEmpty(unitJsonPath) ? "None" : unitJsonPath);
            }
            GUILayout.EndVertical();

            EditorGUILayout.Space();

            // [카드 UI 영역]
            GUILayout.BeginVertical("box");
            convertCard = EditorGUILayout.ToggleLeft(" 2. Card Data 변환하기", convertCard, EditorStyles.boldLabel);
            if (convertCard)
            {
                if (GUILayout.Button("Select Card JSON"))
                    cardJsonPath = EditorUtility.OpenFilePanel("Select Card JSON", "", "json");
                EditorGUILayout.LabelField("Path: ", string.IsNullOrEmpty(cardJsonPath) ? "None" : cardJsonPath);
            }
            GUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // [아이템 UI 영역]
            GUILayout.BeginVertical("box");
            convertItem = EditorGUILayout.ToggleLeft(" 3. Item Data 변환하기", convertItem, EditorStyles.boldLabel);
            if (convertItem)
            {
                if (GUILayout.Button("Select Item JSON"))
                    itemJsonPath = EditorUtility.OpenFilePanel("Select Item JSON", "", "json");
                EditorGUILayout.LabelField("Path: ", string.IsNullOrEmpty(itemJsonPath) ? "None" : itemJsonPath);
            }
            GUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Convert Data & Generate Prefabs", GUILayout.Height(40)))
            {
                ConvertProcess();
            }
        }

        private void ConvertProcess()
        {
            EnsureFolderExists(unitDataPath);
            EnsureFolderExists(cardDataPath);
            EnsureFolderExists(itemDataPath);

            Dictionary<string, UnitDataSO> createdUnitsDict = new Dictionary<string, UnitDataSO>();

            // 1. UnitData 변환 (체크된 경우에만 실행)
            if (convertUnit && !string.IsNullOrEmpty(unitJsonPath))
            {
                string unitJsonText = File.ReadAllText(unitJsonPath);
                List<UnitStatDTO> unitDTOs = JsonConvert.DeserializeObject<List<UnitStatDTO>>(unitJsonText);

                foreach (var dto in unitDTOs)
                {
                    // 유닛 정보
                    string assetName = $"Unit_{dto.ID}_{dto.Name_EN}.asset";
                    string fullPath = $"{unitDataPath}/{assetName}";

                    UnitDataSO unitData = AssetDatabase.LoadAssetAtPath<UnitDataSO>(fullPath);

                    if (unitData != null)
                    {
                        Debug.Log($"[데이터 갱신] {fullPath} 데이터를 덮어씌웁니다.");
                    }
                    else
                    {
                        unitData = ScriptableObject.CreateInstance<UnitDataSO>();
                        AssetDatabase.CreateAsset(unitData, fullPath);
                    }

                    // 스텟 정보
                    assetName = $"Unit_{dto.ID}_{dto.Name_EN}_Stat.asset";
                    fullPath = $"{statDataPath}/{assetName}";

                    UnitStatSO statData = AssetDatabase.LoadAssetAtPath<UnitStatSO>(fullPath);

                    if (statData != null)
                    {
                        Debug.Log($"[데이터 갱신] {fullPath} 스텟 데이터를 덮어씌웁니다.");
                    }
                    else
                    {
                        statData = ScriptableObject.CreateInstance<UnitStatSO>();
                        AssetDatabase.CreateAsset(statData, fullPath);
                    }

                    // 스텟 정보 먼저 처리
                    #region Stat Data Init
                    statData.Defense = dto.Defense;
                    statData.MagicResistance = dto.MagicResistance;
                    statData.AbilityPower = dto.AbilityPower;
                    statData.AttackSpeed = dto.AttackSpeed;
                    statData.Attack_Rage = dto.Attack_Rage;
                    statData.Move_Speed = dto.Move_Speed;
                    //statData.MaxManaPoint = dto.MaxManaPoint;             자원은 스킬 정보로 이전
                    //statData.StartManaPoint = dto.StartManaPoint;
                    statData.ManaRegeneration = dto.ManaRegeneration;

                    statData.statsByStart = new StatByStar[3];
                    statData.statsByStart[0] = new StatByStar { MaxHp = dto.MaxHp_1, AttackDamage = dto.AttackDamage_1 };
                    statData.statsByStart[1] = new StatByStar { MaxHp = dto.MaxHp_2, AttackDamage = dto.AttackDamage_2 };
                    statData.statsByStart[2] = new StatByStar { MaxHp = dto.MaxHp_3, AttackDamage = dto.AttackDamage_3 };
                    #endregion

                    unitData.ID = dto.ID;
                    unitData.Name_KR = dto.Name_KR;
                    unitData.Name_EN = dto.Name_EN;
                    System.Enum.TryParse(dto.Race, out unitData.Race);
                    unitData.Cost = dto.Cost;

                    unitData.statData = statData;

                    unitData.NormalAttack_Type = dto.NormalAttack_Type;
                    unitData.Skill_ID = dto.Skill_ID;

                    unitData.Skill_Prefab = CheckAndCreateSkillPrefab(dto.Name_EN);
                    unitData.NormalAttack_Prefab = CheckAndCreateNormalAttackPrefab(dto.NormalAttack_Type);

                    EditorUtility.SetDirty(statData);
                    createdUnitsDict[dto.ID] = unitData; // 캐싱
                }
            }
            else if (convertCard)
            {
                string[] guids = AssetDatabase.FindAssets("t:UnitDataSO", new[] { unitDataPath });
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    UnitDataSO existingSO = AssetDatabase.LoadAssetAtPath<UnitDataSO>(path);
                    if (existingSO != null && !string.IsNullOrEmpty(existingSO.ID))
                    {
                        createdUnitsDict[existingSO.ID] = existingSO;
                    }
                }
            }

            // 2. CardData 변환 (체크된 경우에만 실행)
            if (convertCard && !string.IsNullOrEmpty(cardJsonPath))
            {
                string cardJsonText = File.ReadAllText(cardJsonPath);
                List<CardDataDTO> cardDTOs = JsonConvert.DeserializeObject<List<CardDataDTO>>(cardJsonText);

                foreach (var dto in cardDTOs)
                {
                    string enName = "Unknown";
                    UnitDataSO linkedUnit = null;

                    if (!string.IsNullOrEmpty(dto.getID) && createdUnitsDict.TryGetValue(dto.getID, out linkedUnit))
                    {
                        enName = linkedUnit.Name_EN;
                    }

                    string assetName = $"Card_{dto.cardID}_{enName}.asset";
                    if (string.IsNullOrEmpty(dto.getID)) assetName = $"Card_{dto.cardID}.asset";

                    string fullPath = $"{cardDataPath}/{assetName}";

                    if (AssetDatabase.LoadAssetAtPath<CardDataSO>(fullPath) != null)
                    {
                        Debug.LogWarning($"[중복 방지] {fullPath} 파일이 이미 존재합니다.");
                        continue;
                    }

                    CardDataSO newCardSO = null;

                    if (dto.cardID.StartsWith("Unit"))
                    {
                        UnitCardDataSO unitCard = ScriptableObject.CreateInstance<UnitCardDataSO>();
                        if (linkedUnit != null) unitCard.unitDataSO = linkedUnit;
                        newCardSO = unitCard;
                    }
                    else if (dto.cardID.StartsWith("Spell"))
                    {
                        SpellCardDataSO spellCard = ScriptableObject.CreateInstance<SpellCardDataSO>();
                        newCardSO = spellCard;
                    }
                    else
                    {
                        newCardSO = ScriptableObject.CreateInstance<CardDataSO>();
                    }

                    newCardSO.cardId = dto.cardID;
                    newCardSO.cardName = dto.cardName;
                    newCardSO.description = dto.description;

                    if (!string.IsNullOrEmpty(dto.cardPrefab))
                    {
                        string prefabPath = dto.cardPrefab;
                        if (!prefabPath.EndsWith(".prefab")) prefabPath += ".prefab";

                        GameObject prefabObj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                        if (prefabObj != null)
                        {
                            CardBase targetScript = prefabObj.GetComponentInChildren<CardBase>(true);

                            if (targetScript != null)
                            {
                                newCardSO.cardPrefab = targetScript;
                            }
                            else
                            {
                                Debug.LogWarning($"[할당 실패] {prefabPath} 프리팹 안에 CardBase (또는 UnitCard/SpellCard 등) 스크립트가 붙어있지 않습니다!");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"[파일 찾기 실패] {prefabPath} 경로에 프리팹이 존재하지 않습니다.");
                        }
                    }

                    AssetDatabase.CreateAsset(newCardSO, fullPath);
                }
            }

            if (convertItem && !string.IsNullOrEmpty(itemJsonPath))
            {
                string itemJsonText = File.ReadAllText(itemJsonPath);
                List<ItemDataDTO> itemDTOs = JsonConvert.DeserializeObject<List<ItemDataDTO>>(itemJsonText);

                foreach (var dto in itemDTOs)
                {
                    // 빈 행은 건너뛰기 (JSON에서 아이템 이름이 없는 경우)
                    if (string.IsNullOrEmpty(dto.itemName)) continue;

                    string assetName = $"Item_{dto.ID}_{dto.itemName_EN}.asset";
                    string fullPath = $"{itemDataPath}/{assetName}";

                    ItemSO itemData = AssetDatabase.LoadAssetAtPath<ItemSO>(fullPath);

                    if (itemData != null)
                    {
                        Debug.Log($"[데이터 갱신] {fullPath} 아이템 데이터를 덮어씌웁니다.");
                    }
                    else
                    {
                        itemData = ScriptableObject.CreateInstance<ItemSO>();
                        AssetDatabase.CreateAsset(itemData, fullPath);
                    }

                    itemData.itemName = dto.itemName;
                    itemData.itemDescription = dto.itemDescription;

                    // 아이콘 할당 (경로가 있을 경우)
                    if (!string.IsNullOrEmpty(dto.icon_Path))
                    {
                        itemData.icon = AssetDatabase.LoadAssetAtPath<Sprite>(dto.icon_Path);
                    }

                    // 스탯 데이터 초기화 후 재할당
                    itemData.modifiers.Clear();

                    AddModifierIfValid(itemData, dto.statType_1, dto.modifierType_1, dto.value_1);
                    AddModifierIfValid(itemData, dto.statType_2, dto.modifierType_2, dto.value_2);
                    AddModifierIfValid(itemData, dto.statType_3, dto.modifierType_3, dto.value_3);
                    AddModifierIfValid(itemData, dto.statType_4, dto.modifierType_4, dto.value_4);

                    // 이펙트 모듈 할당 (경로가 있을 경우)
                    itemData.effectModules.Clear();
                    if (!string.IsNullOrEmpty(dto.effectModules_Path))
                    {
                        ItemEffectSO effectModule = AssetDatabase.LoadAssetAtPath<ItemEffectSO>(dto.effectModules_Path);
                        if (effectModule != null)
                        {
                            itemData.effectModules.Add(effectModule);
                        }
                        else
                        {
                            Debug.LogWarning($"[할당 실패] {dto.effectModules_Path} 경로에서 이펙트 모듈을 찾을 수 없습니다.");
                        }
                    }

                    EditorUtility.SetDirty(itemData);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("완료", "선택된 데이터 변환이 완료되었습니다.", "확인");
        }

        private void AddModifierIfValid(ItemSO itemData, string statTypeStr, string modifierTypeStr, float value)
        {
            if (string.IsNullOrEmpty(statTypeStr) || string.IsNullOrEmpty(modifierTypeStr)) return;

            if (System.Enum.TryParse(statTypeStr, out StatType statType) &&
                System.Enum.TryParse(modifierTypeStr, out ModifierType modifierType))
            {
                itemData.modifiers.Add(new StatModifier
                {
                    statType = statType,
                    modifierType = modifierType,
                    value = value
                });
            }
            else
            {
                Debug.LogWarning($"[변환 오류] 스탯 타입({statTypeStr}) 또는 수치 타입({modifierTypeStr})을 Enum으로 변환할 수 없습니다.");
            }
        }

        private void EnsureFolderExists(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        private SkillBase CheckAndCreateSkillPrefab(string enName)
        {
            string folder = $"Assets/Prefabs/Skill/{enName}Skills";
            EnsureFolderExists(folder);

            string prefabPath = $"{folder}/{enName}Skill.prefab";
            SkillBase prefabObj = AssetDatabase.LoadAssetAtPath<SkillBase>(prefabPath);

            if (prefabObj == null)
            {
                Debug.LogError($"enName의 스킬 프리팹을 찾을 수 없습니다. (경로 : {prefabPath})");
                //GameObject dummyGo = new GameObject($"{enName}Skill");
                //prefabObj = PrefabUtility.SaveAsPrefabAsset(dummyGo, prefabPath);
                //DestroyImmediate(dummyGo);
            }
            return prefabObj;
        }

        private SkillBase CheckAndCreateNormalAttackPrefab(string type)
        {
            string folder = "Assets/Prefabs/Skill/NormalAttack";
            EnsureFolderExists(folder);

            string prefabPath = $"{folder}/{type}_NormalAttack.prefab";
            SkillBase prefabObj = AssetDatabase.LoadAssetAtPath<SkillBase>(prefabPath);

            if (prefabObj == null)
            {
                Debug.LogError($"enName의 기본 공격 프리팹을 찾을 수 없습니다. (경로 : {prefabPath})");
                //GameObject dummyGo = new GameObject($"{type}_NormalAttack");
                //prefabObj = PrefabUtility.SaveAsPrefabAsset(dummyGo, prefabPath);
                //DestroyImmediate(dummyGo);
            }
            return prefabObj;
        }
    }
}
#endif
