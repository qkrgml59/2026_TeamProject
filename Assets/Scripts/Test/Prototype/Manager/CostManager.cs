using UnityEngine;
using UnityEngine.Events;
using Utilitys;

public class CostManager : SingletonMonoBehaviour<CostManager>
{
    [Header("Cost Setting")]
    public int currentCost {  get; private set; }
    public int maxCost { get; private set; } = 10;          // 임시로 최대 코스트 10으로 지정

    public UnityEvent<int, int> OnCostChanged;              // UI에 코스트 변경을 전달할 이벤트 선언

    private void Start()
    {
        InitCost(10, 10);
    }

    public void InitCost(int startCost, int _maxCost)           // 스테이지 시작할 때 사용 할 초기화 매서드
    {
        maxCost = _maxCost;
        currentCost = startCost;
        OnCostChanged?.Invoke(currentCost, maxCost);
    }

    public bool CheckCost(int amount)       // 코스트 양이 충분한지 체크
    {
        return currentCost >= amount;
    }

    public void ConsumeCost(int amount)           // 코스트 소비
    {
        if (!CheckCost(amount)) return;

        currentCost -= amount;
        OnCostChanged?.Invoke(currentCost, maxCost);

        Debug.Log($"{amount} Cost 소모. 현재 : {currentCost} / {maxCost}");
    }

    public void RecoverCost(int amount)
    {
        currentCost += amount;

        if (currentCost > maxCost)
        {
            currentCost = maxCost;      // 현재 코스트가 맥스 값을 넘지 않게
        }

        OnCostChanged?.Invoke(currentCost, maxCost);
        Debug.Log($"{amount} Cost 회복. 현재 : {currentCost} / {maxCost}");
    }
}
