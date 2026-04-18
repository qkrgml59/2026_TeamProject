using System.Collections.Generic;
using UI;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private PackSelectView packView;
    [SerializeField] private DeckDraftView deckView;

    private PakcSelectPresenter packPresenter;
    private DeckDraftPresenter deckPresenter;

    private void Start()
    {
        // Pack 선택 시작
        packPresenter = new PakcSelectPresenter(packView);
        packPresenter.OnComplete += OnPackSelected;
        packPresenter.Init();
    }

    /// <summary>
    /// 카드팩 3개 선택 완료
    /// </summary>
    private void OnPackSelected(List<ThemeType> themes)
    {
        StageManager.Instance.ApplySelectedThemes(themes);

        StartDeckDraft();
    }

    /// <summary>
    /// Deck Draft 시작
    /// </summary>
    private void StartDeckDraft()
    {
        deckPresenter = new DeckDraftPresenter(deckView);
        deckPresenter.OnComplete += OnDeckReady;
        deckPresenter.Init();
    }

    /// <summary>
    /// 덱 세팅 완료 -> 전투 이동
    /// </summary>
    private void OnDeckReady()
    {
        Debug.Log("덱 준비 완료 => 전투 시작");
        // SceneManager.LoadScene("BattleScene");
    }
}
