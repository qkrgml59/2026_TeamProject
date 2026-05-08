using UnityEngine;

public struct ShieldEntry
{
    public Object source;               // 보호막 제공자
    public float amount;                // 보호막 크기
    public float expireTime;            // 만료 시간

    public bool IsExpired => expireTime <= Time.time;

    public ShieldEntry(Object source, float amount, float duration)
    {
        this.source = source;
        this.amount = amount;
        this.expireTime = Time.time + duration;
    }
}
