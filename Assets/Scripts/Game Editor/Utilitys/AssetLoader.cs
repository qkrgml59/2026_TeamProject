#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace GameEditor.Utility
{
    public static class AssetLoader
    {
        /// <summary>
        /// 지정된 폴더에서 특정 타입의 에셋 전부 로드
        /// </summary>
        public static List<T> LoadAll<T>(string folderPath) where T : Object
        {
            List<T> result = new();

            if (string.IsNullOrEmpty(folderPath))
            {
                Debug.LogError("AssetLoader: folderPath is null or empty.");
                return result;
            }

            string[] guids = AssetDatabase.FindAssets(
                $"t:{typeof(T).Name}",
                new[] { folderPath }
            );

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                T asset = AssetDatabase.LoadAssetAtPath<T>(path);

                if (asset != null)
                    result.Add(asset);
            }

            return result;
        }
    }
}
#endif
