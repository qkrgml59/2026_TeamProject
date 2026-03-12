using UnityEngine;
using Utilitys;

namespace Prototype.UI
{
    public class IndicatorManager : SingletonMonoBehaviour<IndicatorManager>
    {
        public HealthBarPresenter HPBarPresenter;
        public DamageTextPresenter DamagePresenter;
    }
}
