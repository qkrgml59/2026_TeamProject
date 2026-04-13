using Game.UI;
using UnityEngine;
using Utilitys;

namespace Prototype.UI
{
    public class IndicatorManager : SingletonMonoBehaviour<IndicatorManager>
    {
        public HealthBarPresenter HPBarPresenter;
        public UnitItemPresenter UnitItemPresenter;
        public DamageTextPresenter DamagePresenter;
    }
}
