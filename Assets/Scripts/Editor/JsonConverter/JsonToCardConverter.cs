#if UNITY_EDITOR
using Item;
using Newtonsoft.Json;
using Prototype.Card;
using Prototype.Card.Item;
using Prototype.Card.Spell;
using Prototype.Card.Unit;
using Spell;
using StatSystem;
using System.Collections.Generic;
using System.IO;
using Unit;
using Unit.Skill;
using UnityEditor;
using UnityEngine;


namespace GameEditor.JsonConverter
{
    public class JsonToCardConverter : EditorWindow
    {
        // 체크박스용 bool 변수 추가
        private bool convertUnit = false;
        private bool convertCard = false;
        private bool convertItem = false;
        private bool convertSpell = false;

        // JSON 파일 경로 저장 변수
        private string unitJsonPath = "";
        private string cardJsonPath = "";
        private string itemJsonPath = "";
        private string spellJsonPath = "";

        // 데이터 저장 경로
        private readonly string unitDataPath = "Assets/GameResources/ScriptableObjects/UnitData";
        private readonly string statDataPath = "Assets/GameResources/ScriptableObjects/StatData";
        private readonly string cardDataPath = "Assets/GameResources/ScriptableObjects/CardData";
        private readonly string itemDataPath = "Assets/GameResources/ScriptableObjects/ItemData";
        private readonly string spellDataPath = "Assets/GameResources/ScriptableObjects/SpellData";

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

            // [스펠 UI 영역]
            GUILayout.BeginVertical("box");
            convertSpell = EditorGUILayout.ToggleLeft(" 4. Spell Data 변환하기", convertSpell, EditorStyles.boldLabel);
            if (convertSpell)
            {
                if (GUILayout.Button("Select Spell JSON"))
                    spellJsonPath = EditorUtility.OpenFilePanel("Select Spell JSON", "", "json");
                EditorGUILayout.LabelField("Path: ", string.IsNullOrEmpty(spellJsonPath) ? "None" : spellJsonPath);
            }
            GUILayout.EndVertical();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // [변환 실행 버튼]
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
            EnsureFolderExists(spellDataPath);

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

            // 2. CardData 변환
            if (convertCard && !string.IsNullOrEmpty(cardJsonPath))
            {
                string cardJsonText = File.ReadAllText(cardJsonPath);
                List<CardDataDTO> cardDTOs = JsonConvert.DeserializeObject<List<CardDataDTO>>(cardJsonText);

                foreach (var dto in cardDTOs)
                {
                    if (string.IsNullOrEmpty(dto.cardID)) continue;

                    string enName = dto.cardName_EN;
                    if (string.IsNullOrEmpty(enName) && !string.IsNullOrEmpty(dto.SO_Path))
                    {
                        string[] parts = dto.SO_Path.Split('_');
                        if (parts.Length > 0) enName = parts[parts.Length - 1];
                    }
                    if (string.IsNullOrEmpty(enName)) enName = "Unknown"; 

                    string assetName = $"Card_{dto.cardID}_{enName}.asset";
                    string fullPath = $"{cardDataPath}/{assetName}";

                    CardDataSO newCardSO = AssetDatabase.LoadAssetAtPath<CardDataSO>(fullPath);
                    bool isNew = false;

                    if (newCardSO == null)
                    {
                        isNew = true;
                        if (dto.cardID.StartsWith("Unit")) newCardSO = ScriptableObject.CreateInstance<UnitCardDataSO>();
                        else if (dto.cardID.StartsWith("Spell")) newCardSO = ScriptableObject.CreateInstance<SpellCardDataSO>();
                        else if (dto.cardID.StartsWith("Item")) newCardSO = ScriptableObject.CreateInstance<ItemCardDataSO>();
                        else newCardSO = ScriptableObject.CreateInstance<UnitCardDataSO>();
                    }

                    newCardSO.cardId = dto.cardID;
                    newCardSO.cardName = dto.cardName;
                    newCardSO.description = dto.description;
                    newCardSO.cost = dto.Cost;

                    // 프리팹 매핑
                    if (!string.IsNullOrEmpty(dto.cardPrefab_Path))
                    {
                        string prefabPath = dto.cardPrefab_Path;
                        if (!prefabPath.EndsWith(".prefab")) prefabPath += ".prefab";
                        GameObject prefabObj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                        if (prefabObj != null) newCardSO.cardPrefab = prefabObj.GetComponentInChildren<CardBase>(true);
                    }

                    if (!string.IsNullOrEmpty(dto.SO_Path))
                    {
                        string soPath = dto.SO_Path;
                        if (!soPath.EndsWith(".asset")) soPath += ".asset";

                        if (newCardSO is UnitCardDataSO unitCard)
                        {
                            UnitDataSO originalUnit = AssetDatabase.LoadAssetAtPath<UnitDataSO>(soPath);
                            if (originalUnit != null) unitCard.unitDataSO = originalUnit;
                        }
                        else if (newCardSO is SpellCardDataSO spellCard)
                        {
                            SpellSO originalSpell = AssetDatabase.LoadAssetAtPath<SpellSO>(soPath);
                            if (originalSpell != null)
                            {
                                spellCard.spellSO = originalSpell;
                                spellCard.icon = originalSpell.icon;
                            }
                            if (spellCard.icon == null) spellCard.icon = AutoFindSprite(dto.icon);
                        }
                        else if (newCardSO is ItemCardDataSO itemCard)
                        {
                            ItemSO originalItem = AssetDatabase.LoadAssetAtPath<ItemSO>(soPath);
                            if (originalItem != null)
                            {
                                itemCard.itemSO = originalItem;
                                itemCard.icon = originalItem.icon;
                            }
                            if (itemCard.icon == null) itemCard.icon = AutoFindSprite(dto.icon);
                        }
                    }

                    if (isNew) AssetDatabase.CreateAsset(newCardSO, fullPath);
                    else EditorUtility.SetDirty(newCardSO);
                }
            }

            // 3. ItemData 변환
            if (convertItem && !string.IsNullOrEmpty(itemJsonPath))
            {
                string itemJsonText = File.ReadAllText(itemJsonPath);
                List<ItemDataDTO> itemDTOs = JsonConvert.DeserializeObject<List<ItemDataDTO>>(itemJsonText);

                foreach (var dto in itemDTOs)
                {
                    if (string.IsNullOrEmpty(dto.itemName)) continue;

                    string assetName = $"Item_{dto.ID}_{dto.itemName_EN}.asset";
                    string fullPath = $"{itemDataPath}/{assetName}";

                    ItemSO itemData = AssetDatabase.LoadAssetAtPath<ItemSO>(fullPath);
                    if (itemData == null)
                    {
                        itemData = ScriptableObject.CreateInstance<ItemSO>();
                        AssetDatabase.CreateAsset(itemData, fullPath);
                    }

                    itemData.itemName = dto.itemName;
                    itemData.itemDescription = dto.itemDescription;

                    itemData.icon = AutoFindSprite(dto.icon_Path);
                    if (itemData.icon == null) itemData.icon = AutoFindSprite(dto.itemName_EN);

                    itemData.modifiers.Clear();
                    AddModifierIfValid(itemData, dto.statType_1, dto.modifierType_1, dto.value_1);
                    AddModifierIfValid(itemData, dto.statType_2, dto.modifierType_2, dto.value_2);
                    AddModifierIfValid(itemData, dto.statType_3, dto.modifierType_3, dto.value_3);
                    AddModifierIfValid(itemData, dto.statType_4, dto.modifierType_4, dto.value_4);

                    itemData.effectModules.Clear();
                    if (!string.IsNullOrEmpty(dto.effectModules_Path))
                    {
                        ItemEffectSO effectModule = AssetDatabase.LoadAssetAtPath<ItemEffectSO>(dto.effectModules_Path);
                        if (effectModule != null) itemData.effectModules.Add(effectModule);
                    }

                    EditorUtility.SetDirty(itemData);
                }
            }

            // 4. SpellData 변환
            if (convertSpell && !string.IsNullOrEmpty(spellJsonPath))
            {
                string spellJsonText = File.ReadAllText(spellJsonPath);
                List<SpellDataDTO> spellDTOs = JsonConvert.DeserializeObject<List<SpellDataDTO>>(spellJsonText);

                foreach (var dto in spellDTOs)
                {
                    if (string.IsNullOrEmpty(dto.spellName)) continue;

                    string assetName = $"Spell_{dto.ID}_{dto.spellName_EN}.asset";
                    string fullPath = $"{spellDataPath}/{assetName}";

                    SpellSO spellData = AssetDatabase.LoadAssetAtPath<SpellSO>(fullPath);
                    if (spellData == null)
                    {
                        spellData = ScriptableObject.CreateInstance<SpellSO>();
                        AssetDatabase.CreateAsset(spellData, fullPath);
                    }

                    spellData.spellName = dto.spellName.Trim();
                    spellData.description = dto.description?.Trim();

                    if (!string.IsNullOrEmpty(dto.targetType) && System.Enum.TryParse(dto.targetType.Trim(), out TeamType teamType))
                    {
                        spellData.tileArea = teamType;
                    }

                    spellData.icon = AutoFindSprite(dto.icon?.Trim());
                    if (spellData.icon == null) spellData.icon = AutoFindSprite(dto.spellName_EN?.Trim());

                    if (!string.IsNullOrEmpty(dto.effect))
                    {
                        string effectPath = dto.effect.Trim();
                        if (!effectPath.EndsWith(".asset"))
                        {
                            effectPath += ".asset";
                        }

                        SpellEffectSO effectSO = AssetDatabase.LoadAssetAtPath<SpellEffectSO>(effectPath);

                        if (effectSO != null)
                        {
                            spellData.effect = effectSO;
                        }
                        else
                        {
                            Debug.LogWarning($"[할당 실패] {effectPath} 경로에서 SpellEffectSO를 찾을 수 없습니다. 경로와 파일명을 다시 확인해주세요.");
                        }
                    }

                    EditorUtility.SetDirty(spellData);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("완료", "선택된 데이터 변환이 완료되었습니다.", "확인");
        }

        private Sprite AutoFindSprite(string searchName)
        {
            if (string.IsNullOrEmpty(searchName)) return null;

            Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>(searchName);
            if (spr != null) return spr;

            string fileName = Path.GetFileNameWithoutExtension(searchName);

            string[] guids = AssetDatabase.FindAssets($"{fileName} t:Sprite");
            if (guids.Length > 0)
            {
                string realPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<Sprite>(realPath);
            }

            return null; 
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
