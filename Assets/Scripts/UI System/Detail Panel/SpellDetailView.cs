using Prototype.Card.Spell;
using Spell;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class SpellDetailView : MonoBehaviour
{
    [Header("아이콘 설정")] public Image iconImage;
    public Sprite defaultIcon;


    [Header("마법 이름")] public TextMeshProUGUI nameTMP;


    [Header("마법 설명")] public TextMeshProUGUI descriptionTMP;

    public void Show(SpellCardDataSO spell)
    {
        if (spell == null || spell.spellSO == null) return;

        if (nameTMP != null) nameTMP.text = spell.spellSO.spellName;

        if (iconImage != null) iconImage.sprite = spell.icon ?? defaultIcon;

        if (descriptionTMP != null) descriptionTMP.text = spell.spellSO.description;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (nameTMP != null) nameTMP.text = "spell name";

        if (iconImage != null) iconImage.sprite = defaultIcon;

        if (descriptionTMP != null) descriptionTMP.text = "spell description";

        gameObject.SetActive(false); ;
    }
}
