using Prototype.Grid;
using System.Collections;
using System.Collections.Generic;
using Unit.Skill;
using UnityEngine;
using UnityEngine.UI;
using Stat;

namespace Unit.Skill
{
    public class PlagueDoctor_Skill : SkillBase
    {
        [Header("스킬 수치")]
        [SerializeField] private float Value_A = 5f;   // 지속시간
        [SerializeField] private float Value_B = 0.2f; // 20% 감소

        private Coroutine skillRoutine;

        protected override void OnStart()
        {
            skillRoutine = StartCoroutine(SkillCast());
        }

        private IEnumerator SkillCast()
        {
            //1.공격 제한
            owner.SetTargetUnit(null);
            //공격 불가 TODO : 나중에... 상태 넣어야 함
            //TODO : 이런 느낌으로 쉴드도 넣어야..owner.AddShield(shieldAmount);
            yield return null;


            //2. 중심 타일 구하기 
            Vector3Int centerCube = HexMath.OffsetToCube(owner.offset);
            HexTile centerTile = GridManager.Instance.GetTile(centerCube);


            //3. 주변 타일 하나씩 직접 가져오기
            List<HexTile> tiles = new List<HexTile>();

            if (centerTile != null)
                tiles.Add(centerTile);
            //주변 6칸
            foreach (var dir in HexMath.CubeDirections)
            {
                HexTile tile = GridManager.Instance.GetTile(centerCube + dir);
                if (tile != null)
                    tiles.Add(tile);
            }


            //4. 디버프 적용
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] == null) continue;

                // TODO: 실제 디버프 효과 넣어야 함
                Debug.Log($"타일 {tiles[i].name} 영향 받음");
            }

            //5. 지속 시간
            yield return new WaitForSeconds(Value_A);

            //6. 종료
            Debug.Log("역병의사 스킬 종료");


            FinishSkill();
        }
        protected override void OnCancel()
        {
            if (skillRoutine != null)
            {
                StopCoroutine(skillRoutine);
                skillRoutine = null;
            }
        }

        protected override void OnFinish()
        {
            if (skillRoutine != null)
            {
                StopCoroutine(skillRoutine);
                skillRoutine = null;
            }
        }
    }
}

