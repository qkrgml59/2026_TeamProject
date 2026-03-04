using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Abiogenesis3d
{
    public enum AutoImportCondition
    {
        Builtin,
        URP,
        URP_Unity6
    }

    [ExecuteInEditMode]
    public class AutoImportUnityPackage : MonoBehaviour
    {
        [Header("This gameObject will be destroyed after import, unless you're in the prefab stage.")]
        public Object unitypackage;
        public AutoImportCondition condition;

        void Awake()
        {
#if UNITY_EDITOR
            bool shouldImport = false;

#if UNITY_PIPELINE_URP
    #if UNITY_6000_0_OR_NEWER
            if (condition == AutoImportCondition.URP_Unity6) shouldImport = true;
    #else
            if (condition == AutoImportCondition.URP) shouldImport = true;
    #endif
#else
        if (condition == AutoImportCondition.Builtin) shouldImport = true;
#endif

            if (shouldImport)
            {
                string path = AssetDatabase.GetAssetPath(unitypackage);
                if (PrefabStageUtility.GetCurrentPrefabStage())
                {
                    Debug.Log("Emulating import in prefab stage: " + path);
                }
                else
                {
                    AssetDatabase.ImportPackage(path, false);
                    Debug.Log("Imported: " + path);
                    DestroyImmediate(gameObject, true);
                }
            }
            #endif
        }
    }
}
