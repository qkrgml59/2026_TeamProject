using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class PlayerCostView : MonoBehaviour
    {
        [Header("UI 요소")]
        public TextMeshProUGUI costText;

        private void Start()
        {
            if (CostManager.Instance != null)
            {
                CostManager.Instance.OnCostChanged.AddListener(UpdateCostText);

                UpdateCostText(CostManager.Instance.currentCost, CostManager.Instance.maxCost);
            }
        }

        private void OnDestroy()
        {
            if (CostManager.Instance != null)
            {
                CostManager.Instance.OnCostChanged.RemoveListener(UpdateCostText);
            }
        }

        private void UpdateCostText(int current, int max)
        {
            if (costText != null)
            {
                costText.text = current.ToString();

                // 현재/최대로 표시하고 싶을 때.
                // costText.text = $"{current} / {max}"; 
            }
        }
    }
}
