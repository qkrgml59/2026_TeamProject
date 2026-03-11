using UnityEngine;
using UnityEngine.UI;
using Prototype.Unit;

public class HealthBarView : MonoBehaviour
{
    public Image currentHPBar;
    RectTransform rectTransform;
    UnitBase _Unit;

    public void Init(UnitBase unit)
    {
        _Unit = unit;
    }

    private void Awake() 
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if(_Unit != null && _Unit.CurFSM != UnitStateType.Dead)
        {
            rectTransform.position = Camera.main.WorldToScreenPoint(_Unit.transform.position + Vector3.up * 0.5f);

            // TODO: 이벤트 형식으로 변경 예정
            UpdateHp(_Unit.currentHp, _Unit.statSet.MaxHp.Value);
        }
    }
    
    public void UpdateHp(float current, float max)
    {
        if(max == 0)
        {
            Debug.LogError("최대 체력은 0이 될 수 없습니다.");
            return;
        }

        currentHPBar.fillAmount = current / max;
    }
}
