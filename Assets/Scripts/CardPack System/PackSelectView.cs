using UnityEngine;
using System.Collections.Generic;
using System;
using Prototype.Card;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    public class PackSelectView : MonoBehaviour
    {
        public System.Action<ThemeType> OnLeftClickPack;
        public System.Action<ThemeType> OnRightClickPack;
        public System.Action OnCloseDetail;
        public System.Action OnConfirm;

        [Header("메인 레이아웃")]
        public RectTransform packListRoot;    // 카드팩 슬롯들이 담긴 부모 오브젝트
        public TextMeshProUGUI countText;
        public GameObject packSlotPrefab;    // 생성할 카드팩 슬롯 프리팹
        public Button confirmButton;

        [Header("상세창 설정")]
        public GameObject detailPanel;
        public Transform detailContentParent;
        public GameObject detailCardPrefab;
        public Image detailPackImage;
        public TextMeshProUGUI detailPackName;


        private Dictionary<ThemeType, PackSlot> slotMap = new();
        private List<GameObject> activeDetailSlots = new List<GameObject>();
        private Vector2 originPos;
        private bool isInitialized = false;

        private void Awake()
        {
            //초기 위치 저장
            originPos = packListRoot.anchoredPosition;
            confirmButton.onClick.AddListener(() => OnConfirm?.Invoke());
        }

        private void Update()
        {
            //ESC를 누르면 닫기
            if (detailPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
                OnCloseDetail?.Invoke();
        }

        ///<summary>
        ///카드 UI슬롯 생성
        /// </summary>
        public void Bind(Dictionary<ThemeType, StageData> stageDict)
        {
            foreach (Transform child in packListRoot)
            {
                Destroy(child.gameObject);
            }
            slotMap.Clear();

            foreach (var pair in stageDict)
            {
                if (packSlotPrefab == null)
                {
                    Debug.LogError("PackSlotPrefab이 연결되지 않았습니다!");
                    return;
                }

                GameObject go = Instantiate(packSlotPrefab, packListRoot);

                if (go.TryGetComponent(out PackSlot slotScript))
                {
                    slotScript.Init(pair.Key, pair.Value, this);
                    slotMap[pair.Key] = slotScript;
                }
            }
        }

        /// <summary>
        /// 특정 팩 슬롯의 선택 하이라이트 상태 변경
        /// </summary>
        public void SetSelectedUI(ThemeType theme, bool isSelected)
        {
            if (slotMap.TryGetValue(theme, out var slot)) slot.SetSelected(isSelected);
        }


        //TODO : 상태창 카드팩이 안 보이는 거 수정 해야 함
        ///<summary>
        ///상세창 연출
        /// </summary>
        public void OpenDetail(ThemeType theme, StageData stageData, List<CardEntry> entries)
        {
            detailPanel.SetActive(true);

            if (detailPackImage != null) detailPackImage.sprite = stageData.Packimage;
            if (detailPackName != null) detailPackName.text = theme.ToString();

            foreach (var s in activeDetailSlots) Destroy(s);
            activeDetailSlots.Clear();

            foreach (var entry in entries)
            {
                if (entry.cardData == null) continue;
                GameObject go = Instantiate(detailCardPrefab, detailContentParent);
                activeDetailSlots.Add(go);
                if (go.TryGetComponent(out PreviewCardSlot slotScript))
                {
                    slotScript.Init(entry.cardData, entry.count);
                }
            }
        }

        ///<summary>
        ///상세창 끄고 다시 복구
        /// </summary>
        public void HideDetail()
        {
            detailPanel.SetActive(false);
        }

        public void UpdateCount(int count)
        {
            countText.text = $"카드팩 선택 : {count} / 3";
            confirmButton.interactable = count >= 3;
        }
    }
}

