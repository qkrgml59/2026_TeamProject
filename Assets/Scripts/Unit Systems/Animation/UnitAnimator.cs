using Prototype.Grid;
using UnityEngine;

namespace Unit.Animation
{

    public class UnitAnimator : MonoBehaviour
    {
        [Header("Default Settings")]
        [SerializeField] private Texture2D dummyImage;
        [SerializeField] private Renderer quadRenderer;
        private MaterialPropertyBlock mpb;


        [Header("Animation Data")]
        public UnitAnimationSO animSO;
        public HexDirectionType currentDirection = HexDirectionType.Left;

        private UnitBase _unit;

        public void Init(UnitBase unit, UnitAnimationSO animSO)
        {
            if (animSO == null) return;

            this.animSO = animSO;

            SetLookDirection(HexDirectionType.Left);       // 좌측으로 초기화

            if(unit != null)
            {
                _unit = unit;
                unit.unitEvents.OnLookDirectionChanged.AddListener(SetLookDirection);
            }
        }

        private void OnDestroy()
        {
            if(_unit != null) _unit.unitEvents.OnLookDirectionChanged.RemoveListener(SetLookDirection);
        }

        public void SetLookDirection(HexDirectionType dirType)
        {
            currentDirection = dirType;
            if (animSO != null)
            {
                switch(dirType)
                {
                    // 우측, 우측 하단은 우측 정면
                    case HexDirectionType.Right:
                    case HexDirectionType.BottomRight:
                        if (animSO.RightFront != null)
                            ApplyVisual(animSO.RightFront.texture);
                        break;

                    // 우측 상단은 좌측 후면
                    case HexDirectionType.TopRight:
                        if (animSO.RightBack != null)
                            ApplyVisual(animSO.RightBack.texture);
                        break;

                    // 좌측, 좌측 하단은 좌측 정면
                    case HexDirectionType.Left:
                    case HexDirectionType.BottomLeft:
                        if (animSO.LeftFront != null)
                            ApplyVisual(animSO.LeftFront.texture);
                        break;

                    // 좌측 상단은 좌측 후면
                    case HexDirectionType.TopLeft:
                        if (animSO.LeftBack != null)
                            ApplyVisual(animSO.LeftBack.texture);
                        break;
                }
            }
        }

        #region Visual Function
        // 유닛 텍스쳐 적용
        public void ApplyVisual(Texture2D texture)
        {
            if (quadRenderer == null || dummyImage == null)
                return;

            // 텍스쳐가 없다면 더미 이미지로 띄워줌
            if (texture == null)
                texture = dummyImage;

            if (mpb == null)
                mpb = new MaterialPropertyBlock();

            quadRenderer.GetPropertyBlock(mpb);
            mpb.SetTexture("_BaseMap", texture);
            quadRenderer.SetPropertyBlock(mpb);
        }
        #endregion

#if UNITY_EDITOR
        private void OnValidate()
        {
            if(animSO != null && animSO.RightFront != null)
                ApplyVisual(animSO.RightFront.texture);
            else if (dummyImage != null)
                ApplyVisual(dummyImage);
        }
#endif
    }

}
