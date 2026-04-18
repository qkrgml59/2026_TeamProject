using UnityEngine;
using System.Collections.Generic;
using UI;

public class PakcSelectPresenter 
{
    private PackSelectView view;
    private List<StageData> packs;
    private List<ThemeType> selected = new();

    public System.Action<List<ThemeType>> OnComplete;

    public PakcSelectPresenter(PackSelectView view)
    {
        this.view = view;

        view.OnLeftClickPack += SelectPack;
        view.OnRightClickPack += ShowDetail;
    }

    public void Init()
    {
        packs = StageManager.Instance.GetStageDataList();
        view.Bind(packs);
    }

    /// <summary>
    /// 좌클릭  카드팩 선택
    /// </summary>
    private void SelectPack(int index)
    {
        if (index < 0 || index >= packs.Count)
            return;

        selected.Add(packs[index].themeType);

        if (selected.Count >= 3)
            OnComplete?.Invoke(selected);
    }

    /// <summary>
    /// 우클릭  카드팩 미리보기
    /// </summary>
    private void ShowDetail(int index)
    {
        if (index < 0 || index >= packs.Count)
            return;

        var cards = StageManager.Instance.GetPreviewCards(packs[index]);
        view.ShowDetail(cards);
    }
}
