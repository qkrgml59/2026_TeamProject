using Prototype.Unit;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.UI
{
    public class HealthBarPresenter : MonoBehaviour
    {
        public HealthBarView HPBarViewPrefab;
        public Canvas HPBarCanvas;

        Dictionary<UnitBase, HealthBarView> viewData = new();

        public void RegisterHealthBar(UnitBase unit)
        {
            if(viewData.ContainsKey(unit))
            {
                viewData[unit].Init(unit);
            }
            else
            {
                // 새로운 Hp바 추가
                if (HPBarViewPrefab == null)
                {
                    Debug.LogWarning("HPBarViewPrefab이 없습니다.", this);
                    return;
                }

                if(HPBarCanvas == null)
                {
                    Debug.LogWarning("HPBarCanvas가 없습니다.", this);
                    return;
                }

                HealthBarView view = Instantiate(HPBarViewPrefab, HPBarCanvas.transform);
                view.Init(unit);
                viewData.Add(unit, view);
            }
        }

        public void RemoveHealthBar(UnitBase unit)
        {

        }
    }
}
