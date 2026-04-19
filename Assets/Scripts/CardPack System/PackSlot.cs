using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// 카드팩 슬롯 입력 처리
    /// </summary>
    public class PackSlot : MonoBehaviour, IPointerClickHandler
    {
        public TextMeshProUGUI nameText;
        public GameObject selectedFrame;
        private ThemeType themeKey;
        private PackSelectView view;

        public void Init(ThemeType theme, StageData data, PackSelectView view)
        {
            themeKey = theme;
            this.view = view;
            nameText.text = theme.ToString();
            selectedFrame.SetActive(false);
        }

        public void SetSelected(bool val) => selectedFrame.SetActive(val);
      

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Debug.Log($"[클릭] {themeKey} 팩 좌클릭됨");
                view.OnLeftClickPack?.Invoke(themeKey);
            }
                
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                Debug.Log($"[클릭] {themeKey} 팩 우클릭됨 (상세보기)");
                view.OnRightClickPack?.Invoke(themeKey);
            }
                
        }
    }

}

