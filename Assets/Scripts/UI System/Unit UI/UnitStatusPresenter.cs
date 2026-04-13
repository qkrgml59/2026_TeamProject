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
                _view.itemView.Init();
                unit.unitEvents.OnItemChanged.AddListener(_view.OnItemChanged);
            }

            // 아이템 초기화
        }

        public void Dispose()
        {
            // 체력 이벤트 제거
            _unit.unitEvents.OnHpChanged.RemoveListener(_view.hpView.OnHpChanged);
            
            // 아이템 이벤트 제거
            _unit.unitEvents.OnItemChanged.RemoveListener(_view.OnItemChanged);


            GameObject.Destroy(_view.gameObject);
        }
    }
}
