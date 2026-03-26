using UnityEngine;

/// <summary>
/// 데미지를 받을 수 있는 대상에 대한 공통 인터페이스
/// </summary>
public interface IDamageable
{

    /// <summary>
    /// 유닛 팀 넘버
    /// Ex) 아군 0팀 / 적 1팀
    /// </summary>
    int TeamID { get; } 

    bool IsDead { get; }

    void TakeDamage(int damage);

}
