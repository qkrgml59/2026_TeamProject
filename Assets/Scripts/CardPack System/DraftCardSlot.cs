using Prototype.Card;
using UnityEngine;

namespace Title.UI
{
    public class DraftCardSlot : MonoBehaviour
    {
        public GameObject selectionUI;

        private PreviewCardSlot previewSlot;

        private void Awake()
        {
            previewSlot = GetComponent<PreviewCardSlot>();

            if (selectionUI != null) selectionUI.SetActive(false);
        }

        public void Init(CardDataSO data, int count)
        {
            if (previewSlot != null) previewSlot.Init(data, count);
        }

        public void SetSelect(bool isSelected)
        {
            if (selectionUI != null)
                selectionUI.SetActive(isSelected);
        
        }
    }

}


