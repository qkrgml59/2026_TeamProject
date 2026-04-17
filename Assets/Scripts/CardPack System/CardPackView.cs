using Prototype.Card;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPackView : MonoBehaviour, IPointerClickHandler
{
    private List<CardDataSO> previewCards;

    private Action<CardPackView> onSelect;
    private Action<CardPackView> onOpenDetail;

    ///<summary>
    ///초기화
    /// </summary>
    public void Init(
        List<CardDataSO> card, Action<CardPackView> onSelectCallback, Action<CardPackView> onDetailCallback)
    {
        previewCards = card;
        onSelect = onSelectCallback;
        onOpenDetail = onDetailCallback;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            onSelect?.Invoke(this);
        }
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            onOpenDetail?.Invoke(this);
        }
    }

    public List<CardDataSO> GetCards()
    {
        return previewCards;
    }



}
