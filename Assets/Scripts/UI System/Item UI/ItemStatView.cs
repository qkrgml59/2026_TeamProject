using StatSystem;
using TMPro;
using UnityEngine;

public class ItemStatView : MonoBehaviour
{
    public TextMeshProUGUI statModifierText;
    
    public void Show(StatModifier modidier)
    {
        if(statModifierText != null)
            statModifierText.text = StatFormatter.ModifierToString(modidier);

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);

        if (statModifierText != null)
            statModifierText.text = "";
    }
}
