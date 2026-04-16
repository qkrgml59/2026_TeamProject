using UnityEngine;

namespace StatSystem
{

    /// <summary>
    /// 스킬 사용을 위한 자원 타입입니다.
    /// </summary>
    public enum ResourceType
    {
        None = 0,           // 자원 사용 하지 않음
        Mana = 1,           // 마나
        Rage = 2,           // 분노 (예시로 만든 특수 스텟)
    }
}
