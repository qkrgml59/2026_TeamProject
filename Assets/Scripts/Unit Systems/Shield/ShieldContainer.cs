using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;


public class ShieldContainer
{
    List<ShieldEntry> shields = new();

    public float Amount => TotalShieldAmount();

    /// <summary>
    /// 보호막 변경 이벤트
    /// </summary>
    public event Action OnShieldChanged;


    /// <summary>
    /// 보호막 제거 이벤트
    /// </summary>
    public event Action<Object> OnShieldRemoved;

    public void ShiledUpdate()
    {
        CleanUpShields();
    }

    /// <summary>
    /// 보호막으로 데미지 흡수를 시도합니다.
    /// </summary>
    /// <returns>남은 데미지 반환</returns>
    public float AbsorbDamage(float damage)
    {
        int i = 0;

        while (i < shields.Count && damage > 0)
        {
            ShieldEntry shield = shields[i];

            // 만료된 보호막은 스킵
            if (shield.IsExpired || shield.amount <= 0)
            {
                i++;
                continue;
            }

            // 현재 보호막으로 흡수 가능한 경우
            if (shield.amount >= damage)
            {
                shield.amount -= damage;
                shields[i] = shield;

                damage = 0;     // 남은 데미지 0으로

                break;
            }

            // 현재 보호막 전부 소모
            damage -= shield.amount;

            shield.amount = 0;
            shields[i] = shield;

            i++;
        }

        return damage;
    }


    /// <summary>
    /// 보호막을 추가하고, 남은 시간에 따라 정렬 합니다.
    /// </summary>
    public void AddShield(ShieldEntry newShield)
    {
        // 남은 시간 우선 정렬
        int index = shields.FindIndex(s => s.expireTime > newShield.expireTime);

        if (index < 0)
            shields.Add(newShield);
        else
            shields.Insert(index, newShield);


        OnShieldChanged.Invoke();
    }

    public void Clear()
    {
        shields.Clear();
    }


    // 만료 보호막 제거
    void CleanUpShields()
    {
        bool isChanged = false;
        for (int i = shields.Count - 1; i >= 0; i--)
        {
            if (shields[i].IsExpired || shields[i].amount <= 0)
            {
                OnShieldRemoved?.Invoke(shields[i].source);
                shields.RemoveAt(i);            // 만료 되었거나 남은 값이 0 이하일 경우 제거
                isChanged = true;
            }
        }

        if(isChanged) OnShieldChanged.Invoke();
    }

    float TotalShieldAmount()
    {
        float totalShieldAmount = 0;

        int i = 0;
        for (i = 0; i < shields.Count; i++)
        {
            if (shields[i].IsExpired || shields[i].amount <= 0) continue;

            totalShieldAmount += shields[i].amount;
        }

        return totalShieldAmount;
    }
}
