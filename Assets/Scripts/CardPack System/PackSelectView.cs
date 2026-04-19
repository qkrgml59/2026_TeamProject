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
        public GameObject packSlotPrefab; // [추가] 생성할 카드팩 슬롯 프리팹
        public Button confirmButton;

        [Header("상세창 설정")]
        public GameObject detailPanel;
        public Transform detailContentParent;
        public GameObject detailCardPrefab;

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
            // 상세창 활성화 시 ESC를 누르면 닫기 시도
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

        ///<summary>
        ///상세창 연출
        /// </summary>
        public void OpenDetail(List<CardEntry> entries)
        {
            if (!isInitialized)
            {
                originPos = packListRoot.anchoredPosition;
                isInitialized = true;
            }

            // 1. 목록 이동
            packListRoot.anchoredPosition = new Vector2(originPos.x - 600f, originPos.y);
            detailPanel.SetActive(true);

            // 2. 기존에 떠있던 상세 카드 제거
            foreach (var s in activeDetailSlots) Destroy(s);
            activeDetailSlots.Clear();

            // 3. 새로운 상세 카드 생성
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
            packListRoot.anchoredPosition = originPos;
        }

        public void UpdateCount(int count)
        {
            countText.text = $"카드팩 선택 : {count} / 3";
            confirmButton.interactable = count >= 3;
        }
    }
}

