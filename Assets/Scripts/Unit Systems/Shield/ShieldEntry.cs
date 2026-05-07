using UnityEngine;

public struct ShieldEntry
{
    public float amount;                // 보호막 크기
    public float expireTime;            // 만료 시간

    public bool IsExpired => expireTime <= Time.time;

    public ShieldEntry(float amount, float duration)
    {
        this.amount = amount;
        this.expireTime = Time.time + duration;
    }
}
