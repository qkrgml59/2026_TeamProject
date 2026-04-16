using Unit;
using UnityEngine;
using Utilitys;

namespace Game.UI
{

    public class UnitStatusPresenter
    {
        UnitStatusView _view;
        UnitBase _unit;

        public void Init(UnitBase unit, UnitStatusView view )
        {
            if (unit == null || view == null) return;
            _view = view;
            _unit = unit;

            _view.SetTarget(_unit);

            if (_view.hpView != null)
            {
                _view.hpView.SetTeamColor(_unit.team);
                unit.unitEvents.OnHpChanged.AddListener(_view.hpView.OnHpChanged);
            }

            if(_view.itemView != null)
            {
                _view.itemView.Clear();
                unit.unitEvents.OnItemChanged.AddListener(_view.OnItemChanged);
            }

            if(_view.resourceView != null)
            {
                unit.unitEvents.OnResourceChanged.AddListener(_view.resourceView.OnResourceChanged);
            }
        }

        public void Dispose()
        {
            // 체력 이벤트 제거
            if (_view.hpView != null)
                _unit.unitEvents.OnHpChanged.RemoveListener(_view.hpView.OnHpChanged);

            // 아이템 이벤트 제거
            if (_view.itemView != null)
                _unit.unitEvents.OnItemChanged.RemoveListener(_view.OnItemChanged);

            if (_view.resourceView != null)
                _unit.unitEvents.OnResourceChanged.RemoveListener(_view.resourceView.OnResourceChanged);

            GameObject.Destroy(_view.gameObject);
        }
    }
}
