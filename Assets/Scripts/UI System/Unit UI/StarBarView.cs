using UnityEngine;
using UnityEngine.UI;

public class StarBarView : StarViewBase
{
    [Header("유닛 상태 바")]
    public Image unitStatusBar;             // TODO : 추후엔 성급별 이미지 변경

    [Header("성급별 색상")]
    public Color[] colorByStar = new Color[3];

    public override void SetStar(int star)
    {
        if (star >= colorByStar.Length) return;

        if(unitStatusBar != null) unitStatusBar.color = colorByStar[star];
    }
}
