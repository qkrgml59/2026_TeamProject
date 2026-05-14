using GameEditor.Utility;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Unit.Animation
{

    /// <summary>
    /// 유닛 애니메이션 스프라이트 정보입니다.
    /// </summary>
    [CreateAssetMenu(fileName = "new Unit Animation", menuName = "UnitSO/Animation SO")]
    public class UnitAnimationSO : ScriptableObject
    {
        [Header("유닛 데이터")]
        public UnitDataSO unitData;

        [Header("4방향 이미지")]
        public Sprite LeftBack;
        public Sprite LeftFront;
        public Sprite RightBack;
        public Sprite RightFront;

        // 스프라이트 폴더 정보
        const string spritePath = "Assets/GameResources/Sprites/Unit";

#if UNITY_EDITOR

        [ContextMenu("유닛 데이터와 연결")]
        public void BindUnitData()
        {
            if(unitData != null)
            {
                unitData.animationData = this;
                EditorUtility.SetDirty(unitData);
            }
        }

        /// <summary>
        /// 자동으로 유닛에 맞는 스프라이트를 찾아 적용합니다.
        /// </summary>
        [ContextMenu("유닛 스프라이트 자동 탐색")]
        public void AutoAssignDirectionalSprites()
        {
            ClearSprites();

            if (unitData == null) return;

            string path = $"{spritePath}/{unitData.Name_EN}";
            List<Sprite> sprites = AssetUtility.LoadAll<Sprite>(path);
            foreach (Sprite sprite in sprites)
            {
                int index = sprite.name.LastIndexOf('_');

                if (index < 0)
                    continue;

                string suffix = sprite.name[(index + 1)..];

                switch (suffix)
                {
                    case nameof(LeftBack):
                        LeftBack = sprite;
                        break;

                    case nameof(LeftFront):
                        LeftFront = sprite;
                        break;

                    case nameof(RightBack):
                        RightBack = sprite;
                        break;

                    case nameof(RightFront):
                        RightFront = sprite;
                        break;
                }
            }

            EditorUtility.SetDirty(this);
        }


        void ClearSprites()
        {
            LeftBack = null;
            LeftFront = null;
            RightBack = null;
            RightFront = null;

            EditorUtility.SetDirty(this);
        }
#endif
    }
}
