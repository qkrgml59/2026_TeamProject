using Item;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemDetailView : MonoBehaviour
{
    [Header("아이콘 설정")] public Image iconImage;
    public Sprite defaultIcon;


    [Header("아이템 이름")] public TextMeshProUGUI nameTMP;

    [Header("스텟뷰 설정")] public ItemStatView statViewPrefab;
    public Transform statLayout;
    List<ItemStatView> viewList = new();

    [Header("아이템 설명")] public TextMeshProUGUI descriptionTMP;

    public void Show(ItemSO item)
    {
        if (item == null) return;

        if (nameTMP != null) nameTMP.text = item.itemName;

        if (iconImage != null) iconImage.sprite = item.icon ?? defaultIcon;

        SetModifierView(item);

        if (descriptionTMP != null) descriptionTMP.text = item.itemDescription;

        gameObject.SetActive(true);
    }

    void SetModifierView(ItemSO item)
    {
        if(statViewPrefab != null && statLayout != null)
            while (item.modifiers.Count > viewList.Count)                       // 부족한 view 생성
            {
                ItemStatView view = Instantiate(statViewPrefab, statLayout);

                // Stat의 가장 아래쪽에 생성
                view.transform.SetSiblingIndex(viewList.Count);

                viewList.Add(view);
            }


        // 2. 정렬 (enum 순서 기준)
        var sortedModifiers = item.modifiers
        .OrderBy(mod => mod.statType)
        .ToList();

        for(int i = 0; i < viewList.Count; i++)
        {
            if(i >= item.modifiers.Count)
            {
                viewList[i]?.Hide();
                continue;
            }

            viewList[i]?.Show(item.modifiers[i]);
        }
    }

    public void Hide()
    {
        if (nameTMP != null) nameTMP.text = "item name";

        if (iconImage != null) iconImage.sprite = defaultIcon;

        for (int i = 0; i < viewList.Count; i++)
        { 
            viewList[i]?.Hide();
        }

        if (descriptionTMP != null) descriptionTMP.text = "item description";

        gameObject.SetActive(false);
    }
}
