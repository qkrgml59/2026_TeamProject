using UnityEngine;

namespace Spell
{
    public abstract class SpellEffectSO : ScriptableObject
    {

        /// </summary>
        /// RaycastHit 그리드 타일 검사후 실행
        /// </summary>
        /// <returns> 스펠 발동 성공 시 bool값으로 체크 후 반환.</returns>
        public abstract bool TryExecute(RaycastHit hit);

        public abstract string GetDescription();

    }
}

