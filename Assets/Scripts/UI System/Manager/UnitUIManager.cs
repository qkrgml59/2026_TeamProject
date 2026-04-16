using Game.UI;
using System.Collections.Generic;
using Unit;
using UnityEngine;
using Utilitys;

public class UnitUIManager : SingletonMonoBehaviour<UnitUIManager>
{
    [Header("UI Settings")]
    [SerializeField] private UnitStatusView statusViewPrefab;
    [SerializeField] private Canvas statusViewCanvas;

    private Dictionary<UnitBase, UnitStatusPresenter> statusPresenterDic = new();

    public UnitStatusPresenter Create(UnitBase unit)
    {
        if (statusViewPrefab == null || statusViewCanvas == null) return null;

        if (statusPresenterDic.TryGetValue(unit, out UnitStatusPresenter value))
            return value;

        var view = Instantiate(statusViewPrefab, statusViewCanvas.transform);

        var presenter = new UnitStatusPresenter();
        presenter.Init(unit, view);

        statusPresenterDic.Add(unit, presenter);
        unit.unitEvents.OnDestroyedUnit.AddListener(HandleUnitDeath);

        return presenter;
    }


    /// <summary>
    /// 유닛 파괴 이벤트가 발생하면 제거
    /// </summary>
    public void HandleUnitDeath(UnitBase unit)
    {
        if (statusPresenterDic.TryGetValue(unit, out var presenter))
        {
            presenter.Dispose();
            statusPresenterDic.Remove(unit);
        }

        unit.unitEvents.OnDestroyedUnit.RemoveListener(HandleUnitDeath);
    }
}
