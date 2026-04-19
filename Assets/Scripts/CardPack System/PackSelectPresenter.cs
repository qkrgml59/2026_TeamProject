using UnityEngine;
using System.Collections.Generic;
using UI;
using Prototype.Card;

public class PackSelectPresenter 
{
    private PackSelectView view;
    private List<ThemeType> selectedThemes = new();

    public System.Action<List<ThemeType>> OnComplete;   //선택 완료시 외부에 알리기

    public PackSelectPresenter(PackSelectView view)
    {
        this.view = view;

        // View 이벤트를 Presenter 메서드에 바인딩
        view.OnLeftClickPack = ToggleTheme;
        view.OnRightClickPack = ShowPreview;

        view.OnCloseDetail = view.HideDetail;
        view.OnConfirm = () => OnComplete?.Invoke(selectedThemes);
        
    }

    public void Init()
    {
        view.Bind(StageManager.Instance.stages);
        view.UpdateCount(0);
    }

    //좌클릭 선택 (있으면 제거 없으면 선택)
    private void ToggleTheme(ThemeType theme)
    {
        if(selectedThemes.Contains(theme))
        {
            selectedThemes.Remove(theme);
            view.SetSelectedUI(theme, false);
        }
        else
        {
            if (selectedThemes.Count >= 3) return;    //최대 3개
            selectedThemes.Add(theme);
            view.SetSelectedUI(theme, true);
        }
        view.UpdateCount(selectedThemes.Count);
    }

    //우클릭 선택 시 카드팩 상세보기
    private void ShowPreview(ThemeType theme)
    {
        if(StageManager.Instance.stages.TryGetValue(theme, out StageData data))
        {
            //제안..? : 나중에 카드가 많아지면 카테고리 별로 나눠서 보여주게 해도 될 거 같아요.
            // 중복 카드는 CardEntry(count)를 통해 하나로 합쳐서 전달
            List<CardEntry> allEntries = new List<CardEntry>();

            allEntries.AddRange(data.UnitCards);
            allEntries.AddRange(data.ItemCards);
            allEntries.AddRange(data.SpellCards);

            view.OpenDetail(allEntries);
        }
    }
    
}
