using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace UI.Round
{
    public class RoundUI : MonoBehaviour
    {
        [Header("텍스트")]
        [SerializeField] private TextMeshProUGUI roundText;
        [SerializeField] private TextMeshProUGUI stateText;
        [SerializeField] private TextMeshProUGUI timerText;

        [Header("시작 버튼")]
        [SerializeField] private Button startButton;

        [SerializeField] private RectTransform startButtonRect;

        [Header("버튼 위치")]
        [SerializeField] private Vector2 showPos;
        [SerializeField] private Vector2 hidePos;

        [Header("슬라이드 속도")]
        [SerializeField] private float moveSpeed = 10f;

        [Header("라운드 바")]
        [SerializeField] private RoundbarUI roundBarUI;

        private float timer;

        // 애니메이션 중복 방지
        private bool isAnimating;

        private void Start()
        {
            //버튼 연결
            startButton.onClick.AddListener(OnClickStart);

            startButton.gameObject.SetActive(true);

            // 시작할 때 버튼 숨김 위치
            startButtonRect.anchoredPosition = hidePos;

            // 준비 상태면 버튼 열기
            ShowStartButton();

            Refresh();
        }

        private void Update()
        {
            Refresh();

            //전투 중일 떄 타이머 감소
            if (BattleManager.currentBattleState != BattleState.Prepare)
            {
                timer -= Time.deltaTime;

                if (timer < 0)
                    timer = 0;

                timerText.text = $"{timer:F0}초";
            }
        }


        //전체 UI 갱신
        public void Refresh()
        {
            if (StageManager.Instance == null) return;

            //현재 스테이지.라운드 표시
            roundText.text =
                $"{StageManager.Instance.CurStageIndex + 1} - " +
                $"{StageManager.Instance.CurRoundIndex + 1}";

            //현재 배틀 상태 표시
            stateText.text =
                $"{BattleManager.currentBattleState}";

            bool isPrepare =
                BattleManager.currentBattleState == BattleState.Prepare;

            if (isPrepare)
            {
                ShowStartButton();
            }

            timerText.gameObject.SetActive(!isPrepare);

            //라운드 바 갱신
            roundBarUI.Refresh();
        }

        //버튼 펼치기
        public void ShowStartButton()
        {
            if (isAnimating)
                return;

            StartCoroutine(SlideButton(showPos));
        }

        //버튼 숨기기
        public void HideStartButton()
        {
            if (isAnimating) return;

            StartCoroutine(SlideButton(hidePos));
        }

        IEnumerator SlideButton(Vector2 targetPos)
        {
            isAnimating = true;

            startButton.gameObject.SetActive(true);

            //이동
            while (Vector2.Distance(
                startButtonRect.anchoredPosition,
                targetPos) > 1f)
            {
                startButtonRect.anchoredPosition =
                    Vector2.Lerp(
                        startButtonRect.anchoredPosition,
                        targetPos,
                        Time.deltaTime * moveSpeed);

                yield return null;
            }

            //정확한 위치 보정
            startButtonRect.anchoredPosition = targetPos;

            //숨김시 버튼 끄기
            if (targetPos == hidePos)
            {
                startButton.gameObject.SetActive(false);
            }

            isAnimating = false;
        }

       void OnClickStart()
        {
            HideStartButton();

            BattleManager.Instance.BattleStart();

            timer = BattleManager.Instance.combatDuration;

            Refresh();
        }
    }
}


