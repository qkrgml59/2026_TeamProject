using Prototype.Card;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreviewCardSlot : MonoBehaviour
{
    public Image cardImage;
    public TextMeshProUGUI countText;
    //TODO : 카드 이름까지 넣기

    /// <summary>
    /// 카드 데이터와 중복 개수 정보를 받아 UI 세팅
    /// </summary>
    public void Init(CardDataSO data, int count)
    {
        cardImage.sprite = data.icon;
        // 개수가 1개 이상인 경우에만 xN 형태로 표시
    }
}
