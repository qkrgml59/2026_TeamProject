#if UNITY_EDITOR
using GameEditor.Utility;
#endif
using UnityEngine;
using System.Collections.Generic;

namespace GameEditor.UnitPlacer
{
    public class UnitPlacer : MonoBehaviour
    {
        [Header("Stage Data Path")]
        public string stageDataPath = "Assets/GameResources/ScriptableObjects/StageData";

        public List<StageData> stageDatas = new();

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            #if UNITY_EDITOR
            stageDatas = AssetLoader.LoadAll<StageData>(stageDataPath);
            #endif
        }
    }
}
