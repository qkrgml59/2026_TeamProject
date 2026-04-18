using Prototype.Card;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 덱 드래프트 UI
/// </summary>
public class DeckDraftView : MonoBehaviour
{
    public Action<int> OnSelectCard;

    public void Show(List<CardDataSO> cards)
    {
        // 10장 UI 표시
    }

    public void Refresh(List<CardDataSO> cards)
    {
        // UI 갱신
    }
}
