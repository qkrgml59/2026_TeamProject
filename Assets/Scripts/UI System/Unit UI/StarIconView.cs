using UnityEngine;

public class StarIconView : StarViewBase
{
    [Header("별 아이콘 원본")]
    public GameObject starRef;

    [Header("최대 별 개수")]
    public int maxStar = 3;

    public Transform content;

    private GameObject[] icons;         // 생성된 별 아이콘들

    private void Start()
    {
        if (starRef == null || content == null) return;

        icons = new GameObject[maxStar];

        for (int i = 0; i < maxStar; i++)
        {
            GameObject star = Instantiate(starRef, content);
            icons[i] = star;
        }
    }

    public override void SetStar(int star)
    {
        for (int i = 0; i < maxStar; i++)
        {
            if (icons[i] != null)
                icons[i].SetActive(i <= star);
        }
    }
}

