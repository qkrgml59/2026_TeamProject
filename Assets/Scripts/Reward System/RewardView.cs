using Prototype.Card;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RewardView : MonoBehaviour
    {
        [Header("UI")]
        public TextMeshProUGUI nameTmp;
        public TextMeshProUGUI descriptionTmp;
        public Image image;

        [Header("Clikcable")]
        public Button rootButton;                 // 카드 전체 버튼

        private int index;
        private Action<int> onClick;

        //데이터 바인딩 + 클릭 이벤트 연결
        //데이터 바인딩 : 애플리케이션의 데이터 원본(Model)과 화면 구성 요소(View)를 연결하여 자동으로 동기화하는 기술
        // 내가 외우려고 적었음.
        public void Bind(CardDataSO data, int index, Action<int> onClick)
        {
            this.index = index;
            this.onClick = onClick;

            gameObject.SetActive(data != null);

            if (data == null)
            {      
                return;
            }

            nameTmp.text = data.cardName;
            descriptionTmp.text = data.description;
            image.sprite = data.icon;

            rootButton.onClick.RemoveAllListeners();
            rootButton.onClick.AddListener(() =>
            {
                this.onClick?.Invoke(this.index);
            });
        }




    }
}
