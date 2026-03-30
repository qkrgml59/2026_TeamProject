using Prototype.Grid;
using Prototype.Skill;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unit.Skill
{
    /// <summary>
    /// 네메시스 스킬
    /// 1. 가장 먼 적 탐색
    /// 2. 해당 적을 내 앞칸으로 끌고 옴
    /// 3. 데미지 적용
    /// 4. 일정 시간 스턴 부여
    /// </summary>
    public class Nemesis_Skill : SkillBase
    {
        [Header("스킬 수치")]
        [SerializeField] private float baseDamage = 100f;
        [SerializeField] private float Value_A = 1.5f;
        [SerializeField] private float Value_B = 1.5f;             //스턴 시간


        [Header("딜레이")]
        [SerializeField] private float preDelay = 0.05f;       //스킬 시작 전 딜레이
        [SerializeField] private float hitDelay = 0.2f;        //끌어온 후 타격까지의 딜레이

        private Coroutine skillRoutine;

        ///<summary>
        ///스킬 시작시 호출
        /// </summary>

        protected override void OnStart()
        {
            skillRoutine = StartCoroutine(SkillCast());
        }

        private IEnumerator SkillCast()
        {
            yield return new WaitForSeconds(preDelay);

            //1.가장 먼 적 탐색 
            UnitBase farTarget = UnitManager.Instance.FindFarthestEnemy(owner);

            if (farTarget == null)
            {
                FinishSkill();
                yield break;
            }

            //타겟 설정
            owner.SetTargetUnit(farTarget);

            //2. 내 앞칸 위치 계산
            Vector3Int dir = HexMath.GetDirectionCube(owner.offset, farTarget.offset);
            Vector3Int cube = HexMath.OffsetToCube(owner.offset);

            HexTile tile = GridManager.Instance.GetTile(cube + dir);

            //3. 앞칸이 막혀있으면 시계방향 빈칸 탐색
            if (tile == null || !tile.CanReserve(farTarget))
            {
                foreach (var d in HexMath.CubeDirections)
                {
                    tile = GridManager.Instance.GetTile(cube + d);

                    //이동 가능 타일 발견시 종료
                    if (tile != null && tile.CanReserve(farTarget))
                        break;
                }
            }

            //이동 간으 타일이 끝까지 없으면 스킬 실패
            if (tile == null || !tile.CanReserve(farTarget))
            {
                FinishSkill();
                yield break;

            }

            //4.대상 끌어오기
            farTarget.EnterTile(tile);    //타일 갱신
            farTarget.transform.position = tile.transform.position;      //위치 이동

            yield return new WaitForSeconds(hitDelay);

            //5.데미지 적용
            DamageInfo info = GetDamageInfo();
            farTarget.ApplyDamage(info);

            //6.스턴 적용 
            farTarget.ChangeUnitState(UnitStateType.Stun);

            yield return new WaitForSeconds(Value_B);

            FinishSkill();
        }
        
        private DamageInfo GetDamageInfo()
        {
            float damage = baseDamage;

            //AD 계수
            damage += owner.statSet.AttackDamage.Value * Value_A;

            // TODO : 스킬의 치명타 여부 아이템 추가
            bool isCrit = false;
            // 치명타 확인
            //bool isCrit = DamageCalculator.RollCritical(owner.statSet.CritChance.Value);
            //if (isCrit)
            //{
            //    // 치명타 발생 시
            //    damage *= (owner.statSet.CritDamage.Value * 0.01f);
            //}

            //피해 증가
            damage *= (100 + owner.statSet.DamageIncrease.Value) * 0.01f;

            return new DamageInfo(
                owner,
                damage,
                DamageType.Physical,
                isCrit
                );

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
