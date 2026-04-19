using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject packPanel;
    [SerializeField] private GameObject deckPanel;

    private PackSelectPresenter packPresenter;
    private DeckDraftPresenter deckPresenter;

    private void Start()
    {
        titlePanel.SetActive(true);
        packPanel.SetActive(false);
        deckPanel.SetActive(false);
    }

    public void OnClickStart()
    {
        titlePanel.SetActive(false);
        packPanel.SetActive(true);

        packPresenter = new PackSelectPresenter(
           packPanel.GetComponent<PackSelectView>()
          );

        packPresenter.OnComplete += OnPackSelected;
        packPresenter.Init();
    }

    private void OnPackSelected(List<ThemeType> themes)
    {
        StageManager.Instance.ApplySelectedThemes(themes);

        packPanel.SetActive(false);
        deckPanel.SetActive(true);

        StartDeckDraft();
    }

    private void StartDeckDraft()
    {
        // 뷰 가져오기
        var draftView = deckPanel.GetComponent<DeckDraftView>();

        //카드 생성
        deckPresenter = new DeckDraftPresenter(draftView);

        deckPresenter.OnComplete += OnDeckReady;

        deckPresenter.StartDraft();
    }

    private void OnDeckReady()
    {
        Debug.Log("전투 시작");
        SceneManager.LoadScene("GridTestScene");
    }
}
